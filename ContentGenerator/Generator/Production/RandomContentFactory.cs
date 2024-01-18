using RandomDataGenerator.FieldOptions;
using RandomDataGenerator.Randomizers;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;

namespace ContentGenerator.Generator.Production;

public interface IRandomContentFactory
{
    IContent CreateContent(ContentFactoryContext context);
}

public class RandomContentFactory(IContentService contentService)
        : IRandomContentFactory
{
    public IContent CreateContent(ContentFactoryContext context)
    {
        FieldOptionsTextWords fieldOptions = new FieldOptionsTextWords() { Seed = context.GeneratorContext.GetSeed() };
        var loremipsumrandomizer = RandomizerFactory.GetRandomizer(fieldOptions);
        var name = loremipsumrandomizer.Generate()!;

        var result = contentService.Create(name, context.Parent.Id, context.TargetContentType, context.UserId);

        context.GeneratorContext.SetSeed(fieldOptions.Seed.Value);
        return result;
    }
}
