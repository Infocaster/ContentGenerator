using Umbraco.Cms.Core.Models;

namespace ContentGenerator.Generator.Enrichment;

public interface IPropertyFiller
{
    IPropertySink FillProperties(IPropertySink content, IGeneratorContext context);
}

public interface IReusablePropertyFiller
    : IPropertyFiller
{
    IPropertyFiller Reuse(IPropertyType propertyType);
}

public interface IPropertyFillerFactory
{
    ValueTask<IReadOnlyCollection<IPropertyFiller>> CreateAsync(PropertyFillerContext context);
}