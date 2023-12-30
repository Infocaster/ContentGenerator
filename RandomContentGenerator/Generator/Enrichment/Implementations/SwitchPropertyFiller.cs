using Umbraco.Cms.Core.Models;

namespace RandomContentGenerator.Generator.Enrichment.Implementations;

public class SwitchPropertyFillerFactory
    : PropertyFillerFactoryBase
{
    public SwitchPropertyFillerFactory()
        : base("Umbraco.TrueFalse")
    {
    }

    protected override ValueTask<IPropertyFiller> CreateFillerAsync(IPropertyType propertyType, PropertyFillerContext context)
        => ValueTask.FromResult(CreateFiller(propertyType, context));
        
    private IPropertyFiller CreateFiller(IPropertyType propertyType, PropertyFillerContext context)
    {
        return new SwitchPropertyFiller(propertyType);
    }
}

public class SwitchPropertyFiller(IPropertyType propertyType)
        : IReusablePropertyFiller
{
    public IPropertySink FillProperties(IPropertySink content, IGeneratorContext context)
    {
        var rng = context.GetRandom();
        var switchValue = rng.NextDouble() < 0.5 ? "0" : "1";

        content.SetValue(propertyType.Alias, switchValue, null, null);
        return content;
    }

    public IPropertyFiller Reuse(IPropertyType propertyType)
    {
        return new SwitchPropertyFiller(propertyType);
    }
}