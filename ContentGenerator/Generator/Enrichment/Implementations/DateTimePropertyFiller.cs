/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
using Umbraco.Cms.Core.Models;

namespace ContentGenerator.Generator.Enrichment.Implementations;

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
    : IReusablePropertyFiller
{
    public IPropertySink FillProperties(IPropertySink content, IGeneratorContext context)
    {
        var now = DateTime.UtcNow;

        var minDate = now.AddYears(-30).Ticks;
        var maxDate = now.AddYears(30).Ticks;

        var rnd = context.GetRandom();
        var randomDate = new DateTime(rnd.NextInt64(minDate, maxDate));

        var value = randomDate.ToString("yyyy-MM-dd hh:mm:ss");
        content.SetValue(propertyType.Alias, value, null, null);

        return content;
    }

    public IPropertyFiller Reuse(IPropertyType propertyType)
    {
        return new DateTimePropertyFiller(propertyType);
    }
}