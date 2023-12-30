using Umbraco.Cms.Core.Models;

namespace RandomContentGenerator.Generator.Enrichment;

public record PropertyFillerContext
{
    public PropertyFillerContext(IContent parent, IContentType contentType, IFillerCollection fillerCollection, IDictionary<int, IReusablePropertyFiller>? reusableFillers = null)
    {
        Parent = parent;
        ContentType = contentType;
        FillerCollection = fillerCollection;
        Properties = ContentType.CompositionPropertyTypes
            .Union(ContentType.NoGroupPropertyTypes)
            .Union(ContentType.PropertyTypes)
            .ToList();
        ReusableFillers = reusableFillers ?? new Dictionary<int, IReusablePropertyFiller>();
    }

    public PropertyFillerContext(IContent parent, IContentType contentType, PropertyFillerContext original)
        : this(parent, contentType, original.FillerCollection, original.ReusableFillers)
    {
    }

    public IContent Parent { get; }
    public IContentType ContentType { get; }
    public IFillerCollection FillerCollection { get; }

    public IDictionary<int, IReusablePropertyFiller> ReusableFillers { get; }

    public List<IPropertyType> Properties { get; }

    public IPropertyType? GetByAlias(string alias)
        => Properties.FirstOrDefault(prop => string.Equals(prop.Alias, alias, StringComparison.Ordinal));

    public void Consume(IPropertyType propertyType)
        => Properties.Remove(propertyType);
}