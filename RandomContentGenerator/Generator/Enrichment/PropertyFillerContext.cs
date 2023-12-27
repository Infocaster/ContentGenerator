using Umbraco.Cms.Core.Models;

namespace RandomContentGenerator.Generator.Enrichment;

public record PropertyFillerContext
{
    public PropertyFillerContext(IContent parent, IContentType contentType, IFillerCollection fillerCollection)
    {
        Parent = parent;
        ContentType = contentType;
        FillerCollection = fillerCollection;
        Properties = ContentType.CompositionPropertyTypes
            .Union(ContentType.NoGroupPropertyTypes)
            .Union(ContentType.PropertyTypes)
            .ToList();
    }

    public IContent Parent { get; }
    public IContentType ContentType { get; }
    public IFillerCollection FillerCollection { get; }
    public List<IPropertyType> Properties { get; }

    public IPropertyType? GetByAlias(string alias)
        => Properties.FirstOrDefault(prop => string.Equals(prop.Alias, alias, StringComparison.Ordinal));

    public void Consume(IPropertyType propertyType)
        => Properties.Remove(propertyType);
}