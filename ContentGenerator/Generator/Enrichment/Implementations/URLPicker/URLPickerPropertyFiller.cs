/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Services;

namespace ContentGenerator.Generator.Enrichment.Implementations.URLPicker;

public class URLPickerPropertyFillerFactory(IDataTypeService dataTypeService, IEnumerable<IURLPickerLinkGenerator> linkGeneratorList, IJsonSerializer jsonSerializer)
    : PropertyFillerFactoryBase("Umbraco.MultiUrlPicker")
{
    protected override ValueTask<IPropertyFiller> CreateFillerAsync(IPropertyType propertyType, PropertyFillerContext context)
        => ValueTask.FromResult<IPropertyFiller>(CreateFiller(propertyType));

    private URLPickerPropertyFiller CreateFiller(IPropertyType propertyType)
    {
        var config = propertyType.ConfigurationAs<MultiUrlPickerConfiguration>(dataTypeService);

        // Since mandatory properties cannot contain 0 items,
        //    the minimum for this property is always at least 1
        var min = Math.Max(config.MinNumber, 1);

        var max = config.MaxNumber;
        if (max == default) max = min + 10;

        return new URLPickerPropertyFiller(propertyType, min..(max + 1), linkGeneratorList.ToList(), jsonSerializer);
    }
}

public class URLPickerPropertyFiller(IPropertyType propertyType, Range sizeRange, IReadOnlyList<IURLPickerLinkGenerator> linkGeneratorList, IJsonSerializer jsonSerializer)
    : IReusablePropertyFiller
{
    private delegate MultiUrlPickerValueEditor.LinkDto LinkGenerator(IGeneratorContext context);

    public IPropertySink FillProperties(IPropertySink content, IGeneratorContext context)
    {
        var rnd = context.GetRandom();

        var amount = rnd.Next(sizeRange.Start.Value, sizeRange.End.Value);

        List<MultiUrlPickerValueEditor.LinkDto> result = new(amount);

        for (int i = 0; i < amount; i++)
        {
            var generator = linkGeneratorList[rnd.Next(linkGeneratorList.Count)];
            result.Add(generator.GenerateLink(context));
        }

        content.SetValue(propertyType.Alias, jsonSerializer.Serialize(result), null, null);
        return content;
    }

    public IPropertyFiller Reuse(IPropertyType propertyType)
    {
        return new URLPickerPropertyFiller(propertyType, sizeRange, linkGeneratorList, jsonSerializer);
    }
}
