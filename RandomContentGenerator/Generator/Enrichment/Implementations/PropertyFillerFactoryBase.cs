
using Umbraco.Cms.Core.Models;

namespace RandomContentGenerator.Generator.Enrichment.Implementations;

public abstract class PropertyFillerFactoryBase(string propertyTypeAlias, bool reuseFiller = true)
        : IPropertyFillerFactory
{
    public async ValueTask<IReadOnlyCollection<IPropertyFiller>> CreateAsync(PropertyFillerContext context)
    {
        var properties = context.Properties.Where(p => string.Equals(p.PropertyEditorAlias, propertyTypeAlias)).ToList();
        List<IPropertyFiller> fillers = new (properties.Count);
        foreach(var p in properties)
        {
            IPropertyFiller? filler;
            if (context.ReusableFillers.TryGetValue(p.DataTypeId, out var reusableFiller))
            {
                filler = reusableFiller.Reuse(p);
            }
            else
            {
                filler = await CreateFillerAsync(p, context);
                if (reuseFiller && filler is IReusablePropertyFiller f) context.ReusableFillers.Add(p.DataTypeId, f);
            }

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
