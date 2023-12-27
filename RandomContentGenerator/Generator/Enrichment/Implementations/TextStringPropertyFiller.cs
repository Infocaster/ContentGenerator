using RandomDataGenerator.FieldOptions;
using RandomDataGenerator.Randomizers;
using Umbraco.Cms.Core.Models;

namespace RandomContentGenerator.Generator.Enrichment.Implementations;

public class TextStringPropertyFillerFactory
    : PropertyFillerFactoryBase
{
    public TextStringPropertyFillerFactory()
        : base("Umbraco.TextBox")
    {
    }

    protected override ValueTask<IPropertyFiller> CreateFillerAsync(IPropertyType propertyType, PropertyFillerContext context)
        => ValueTask.FromResult(CreateFiller(propertyType, context));
        
    private IPropertyFiller CreateFiller(IPropertyType propertyType, PropertyFillerContext context)
    {
        return new TextStringPropertyFiller(propertyType);
    }
}

public class TextStringPropertyFiller(IPropertyType propertyType)
        : IPropertyFiller
{
    public IPropertySink FillProperties(IPropertySink content, IGeneratorContext context)
    {
        var loremipsumrandomizer = RandomizerFactory.GetRandomizer(new FieldOptionsTextWords() { Seed = context.GetRandom().Next() });
        var text = loremipsumrandomizer.Generate();

        content.SetValue(propertyType.Alias, text, null, null);
        return content;
    }
}