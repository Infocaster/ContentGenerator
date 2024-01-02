using ContentGenerator.Generator;
using ContentGenerator.Generator.Enrichment;
using ContentGenerator.Generator.Production;
using Umbraco.Cms.Core.Services;

namespace ContentGenerator.Request;

public record GenerateContentRequest(
    int ContentId,
    int? Seed,
    int Amount,
    IReadOnlyCollection<int> ContentTypes,
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

public class GenerateContentRequestHandler : IGenerateContentRequestHandler
{
    private readonly IContentService contentService;
    private readonly IContentTypeService contentTypeService;
    private readonly IRandomContentFactory randomContentFactory;
    private readonly IRandomContentEnricherFactory enricherFactory;

    public GenerateContentRequestHandler(
        IContentService contentService,
        IContentTypeService contentTypeService,
        IRandomContentFactory randomContentFactory,
        IRandomContentEnricherFactory enricherFactory)
    {
        this.contentService = contentService;
        this.contentTypeService = contentTypeService;
        this.randomContentFactory = randomContentFactory;
        this.enricherFactory = enricherFactory;
    }

    public async ValueTask HandleAsync(GenerateContentRequest request, GenerateContentContext context)
    {
        if (request.Amount < 1) return;

        var content = contentService.GetById(request.ContentId)
            ?? throw new InvalidOperationException("Cannot generate content below the node with the given id");

        var targetContentTypes = contentTypeService.GetAll(request.ContentTypes.ToArray()).ToList();

        var generatorContext = CreateGeneratorContext(request);

        var rnd = generatorContext.GetRandom();

        for (int i = 0; i < request.Amount; i++)
        {
            var targetContentType = targetContentTypes[rnd.Next(targetContentTypes.Count)];
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