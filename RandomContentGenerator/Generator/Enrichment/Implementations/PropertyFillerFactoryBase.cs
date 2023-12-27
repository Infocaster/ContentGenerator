
using Umbraco.Cms.Core.Models;

namespace RandomContentGenerator.Generator.Enrichment.Implementations;

public abstract class PropertyFillerFactoryBase(string propertyTypeAlias)
        : IPropertyFillerFactory
{
    public async ValueTask<IReadOnlyCollection<IPropertyFiller>> CreateAsync(PropertyFillerContext context)
    {
        var properties = context.Properties.Where(p => string.Equals(p.PropertyEditorAlias, propertyTypeAlias)).ToList();
        List<IPropertyFiller> fillers = new (properties.Count);
        foreach(var p in properties)
        {
            var filler = await CreateFillerAsync(p, context);
            if (!p.Mandatory)
            {
                filler = filler.MakeOptional();
            }

            fillers.Add(filler);
        }

        foreach(var p in properties) context.Consume(p);

        return fillers;
    }

    protected abstract ValueTask<IPropertyFiller> CreateFillerAsync(IPropertyType propertyType, PropertyFillerContext context);
}
