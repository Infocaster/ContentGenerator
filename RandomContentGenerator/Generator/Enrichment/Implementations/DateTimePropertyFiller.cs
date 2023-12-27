using Umbraco.Cms.Core.Models;

namespace RandomContentGenerator.Generator.Enrichment.Implementations;

public class DateTimePropertyFillerFactory()
    : PropertyFillerFactoryBase("Umbraco.DateTime")
{
    protected override ValueTask<IPropertyFiller> CreateFillerAsync(IPropertyType propertyType, PropertyFillerContext context)
        => ValueTask.FromResult(CreateFiller(propertyType, context));
        
    private IPropertyFiller CreateFiller(IPropertyType propertyType, PropertyFillerContext context)
    {
        return new DateTimePropertyFiller(propertyType);
    }
}

public class DateTimePropertyFiller(IPropertyType propertyType)
    : IPropertyFiller
{
    public IPropertySink FillProperties(IPropertySink content, IGeneratorContext context)
    {
        var minDate = new DateTime(2000, 1, 1).Ticks;
        var maxDate = new DateTime(2070, 1, 1).Ticks;

        var rnd = context.GetRandom();
        var randomDate = new DateTime(rnd.NextInt64(minDate, maxDate));

        var value = randomDate.ToString("yyyy-MM-dd hh:mm:ss");
        content.SetValue(propertyType.Alias, value, null, null);

        return content;
    }
}