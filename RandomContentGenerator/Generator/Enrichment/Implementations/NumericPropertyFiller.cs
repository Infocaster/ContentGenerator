using Umbraco.Cms.Core.Models;

namespace RandomContentGenerator.Generator.Enrichment.Implementations;

public class NumericPropertyFillerFactory()
        : PropertyFillerFactoryBase("Umbraco.Integer")
{
    protected override ValueTask<IPropertyFiller> CreateFillerAsync(IPropertyType propertyType, PropertyFillerContext context)
        => ValueTask.FromResult(CreateFiller(propertyType, context));
        
    private IPropertyFiller CreateFiller(IPropertyType propertyType, PropertyFillerContext context)
    {
        return new NumericPropertyFiller(propertyType);
    }
}

public class NumericPropertyFiller(IPropertyType propertyType)
        : IPropertyFiller
{
    public IPropertySink FillProperties(IPropertySink content, IGeneratorContext context)
    {
        Random rnd = context.GetRandom();

        content.SetValue(propertyType.Alias, rnd.Next(0, 100), null, null);

        return content;
    }
}
