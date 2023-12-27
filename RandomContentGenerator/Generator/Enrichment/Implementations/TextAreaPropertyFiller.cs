using RandomDataGenerator.FieldOptions;
using RandomDataGenerator.Randomizers;
using Umbraco.Cms.Core.Models;

namespace RandomContentGenerator.Generator.Enrichment.Implementations;

public class TextAreaPropertyFillerFactory
    : PropertyFillerFactoryBase
{
    public TextAreaPropertyFillerFactory()
    : base("Umbraco.TextArea")
    {
    }

    protected override ValueTask<IPropertyFiller> CreateFillerAsync(IPropertyType propertyType, PropertyFillerContext context)
        => ValueTask.FromResult(CreateFiller(propertyType, context));
        
    private IPropertyFiller CreateFiller(IPropertyType propertyType, PropertyFillerContext context)
    {
        return new TextAreaPropertyFiller(propertyType);
    }
}

public class TextAreaPropertyFiller(IPropertyType propertyType)
        : IPropertyFiller
{
    public IPropertySink FillProperties(IPropertySink content, IGeneratorContext context)
    {
        var rng = context.GetRandom();
        FieldOptionsTextLipsum fieldOptions = new ()
        {
            Paragraphs = rng.Next(1, 5),
            Seed = rng.Next()
        };
        var loremipsumgenerator = RandomizerFactory.GetRandomizer(fieldOptions);

        var text = loremipsumgenerator.Generate();
        content.SetValue(propertyType.Alias, text, null, null);

        return content;
    }
}
