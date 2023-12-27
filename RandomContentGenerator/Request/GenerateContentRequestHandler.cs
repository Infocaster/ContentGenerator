using RandomContentGenerator.Generator;
using RandomContentGenerator.Generator.Enrichment;
using RandomContentGenerator.Generator.Production;
using Umbraco.Cms.Core.Services;

namespace RandomContentGenerator.Request;

public record GenerateContentRequest(
    int ContentId,
    int? Seed,
    int Amount,
    double OptionalRatio = 0.5,
    Dictionary<string, object>? AdditionalProperties = null
);

public record GenerateContentContext(
    int UserId
);

public interface IGenerateContentRequestHandler
{
    ValueTask HandleAsync(GenerateContentRequest request, GenerateContentContext context);
}

public class GenerateContentRequestHandler(
    IContentService contentService,
    IContentTypeService contentTypeService,
    IRandomContentFactory randomContentFactory,
    IRandomContentEnricherFactory enricherFactory)
    : IGenerateContentRequestHandler
{
    public async ValueTask HandleAsync(GenerateContentRequest request, GenerateContentContext context)
    {
        if (request.Amount < 1) return;

        var content = contentService.GetById(request.ContentId)
            ?? throw new InvalidOperationException("Cannot generate content below the node with the given id");

        var contentType = contentTypeService.Get(content.ContentTypeId)
            ?? throw new InvalidOperationException("Cannot generate content because the given node has an unknown content type");

        var targetContentTypeId = contentType.AllowedContentTypes?.MinBy(ct => ct.SortOrder)
            ?? throw new InvalidOperationException("Cannot generate content because the given node doesn't allow childnodes");

        var targetContentType = contentTypeService.Get(targetContentTypeId.Id.Value)
            ?? throw new InvalidOperationException("Cannot generate content because the childnode type does not exist");

        var generatorContext = CreateGeneratorContext(request);

        for (int i = 0; i < request.Amount; i++)
        {
            var newContent = randomContentFactory.CreateContent(new RandomContentFactoryContext(
                content,
                targetContentType,
                context.UserId,
                generatorContext
            ));

            var enrichers = await enricherFactory.CreateAsync(new RandomContentEnricherContext(
                content,
                targetContentType
            ));

            foreach (var enricher in enrichers) newContent = enricher.Enrich(newContent, generatorContext);

            contentService.SaveAndPublish(newContent, userId: context.UserId);
        }
    }

    private static GeneratorContext CreateGeneratorContext(GenerateContentRequest request)
    {
        var result = new GeneratorContext();
        result.SetSeed(request.Seed ?? Random.Shared.Next());
        result.SetOptionalRatio(request.OptionalRatio);

        if (request.AdditionalProperties is not null) result.Merge(request.AdditionalProperties);

        return result;
    }
}