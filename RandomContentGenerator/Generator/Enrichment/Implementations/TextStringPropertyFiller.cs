using System.Text;
using RandomDataGenerator.FieldOptions;
using RandomDataGenerator.Randomizers;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;

namespace RandomContentGenerator.Generator.Enrichment.Implementations;

public class TextStringPropertyFillerFactory(IDataTypeService dataTypeService)
        : PropertyFillerFactoryBase("Umbraco.TextBox")
{
    protected override ValueTask<IPropertyFiller> CreateFillerAsync(IPropertyType propertyType, PropertyFillerContext context)
        => ValueTask.FromResult(CreateFiller(propertyType, context));
        
    private IPropertyFiller CreateFiller(IPropertyType propertyType, PropertyFillerContext context)
    {
        var config = propertyType.ConfigurationAs<TextboxConfiguration>(dataTypeService);

        var max = config.MaxChars ?? 400;

        return new TextStringPropertyFiller(propertyType, max);
    }
}

public class TextStringPropertyFiller(IPropertyType propertyType, int max)
        : IPropertyFiller
{
    public IPropertySink FillProperties(IPropertySink content, IGeneratorContext context)
    {
        var rnd = context.GetRandom();

        var targetCharacterCount = rnd.Next(1, max + 1);

        var loremipsumrandomizer = RandomizerFactory.GetRandomizer(new FieldOptionsTextWords() { Seed = rnd.Next(), Min = 1, Max = 1 });
        StringBuilder sb = new (max);
        while (sb.Length < targetCharacterCount)
        {
            sb.Append(loremipsumrandomizer.Generate());
            sb.Append(' ');
        }

        sb.Length = targetCharacterCount;

        content.SetValue(propertyType.Alias, sb.ToString().Trim(), null, null);
        return content;
    }
}