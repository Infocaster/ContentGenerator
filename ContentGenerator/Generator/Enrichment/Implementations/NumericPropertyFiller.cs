/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
using Umbraco.Cms.Core.Models;

namespace ContentGenerator.Generator.Enrichment.Implementations;

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
        : IReusablePropertyFiller
{
    public IPropertySink FillProperties(IPropertySink content, IGeneratorContext context)
    {
        Random rnd = context.GetRandom();

        content.SetValue(propertyType.Alias, rnd.Next(0, 100), null, null);

        return content;
    }

    public IPropertyFiller Reuse(IPropertyType propertyType)
    {
        return new NumericPropertyFiller(propertyType);
    }
}
