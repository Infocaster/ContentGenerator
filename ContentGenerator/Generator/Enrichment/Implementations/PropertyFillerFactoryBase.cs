/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
using Umbraco.Cms.Core.Models;

namespace ContentGenerator.Generator.Enrichment.Implementations;

public abstract class PropertyFillerFactoryBase(string propertyTypeAlias, bool reuseFiller = true)
        : IPropertyFillerFactory
{
    public async ValueTask<IReadOnlyCollection<IPropertyFiller>> CreateAsync(PropertyFillerContext context)
    {
        var properties = context.Properties.Where(p => string.Equals(p.PropertyEditorAlias, propertyTypeAlias)).ToList();
        List<IPropertyFiller> fillers = new(properties.Count);
        foreach (var p in properties)
        {
            IPropertyFiller? filler = await GetFillerAsync(context, p);

            if (!p.Mandatory)
            {
                filler = filler.MakeOptional();
            }

            fillers.Add(filler);
            context.Consume(p);
        }

        return fillers;
    }

    private async Task<IPropertyFiller> GetFillerAsync(PropertyFillerContext context, IPropertyType propertyType)
    {
        if (context.ReusableFillers.TryGetValue(propertyType.DataTypeId, out var reusableFiller))
        {
            return reusableFiller.Reuse(propertyType);
        }

        var filler = await CreateFillerAsync(propertyType, context);
        if (reuseFiller && filler is IReusablePropertyFiller f) context.ReusableFillers.Add(propertyType.DataTypeId, f);

        return filler;
    }

    protected abstract ValueTask<IPropertyFiller> CreateFillerAsync(IPropertyType propertyType, PropertyFillerContext context);
}
