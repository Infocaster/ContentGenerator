/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
using System.Text;
using RandomDataGenerator.FieldOptions;
using RandomDataGenerator.Randomizers;
using Umbraco.Cms.Core.Models;

namespace ContentGenerator.Generator.Enrichment.Implementations;

public class RTEPropertyFillerFactory()
    : PropertyFillerFactoryBase("Umbraco.TinyMCE")
{
    protected override ValueTask<IPropertyFiller> CreateFillerAsync(IPropertyType propertyType, PropertyFillerContext context)
        => ValueTask.FromResult(CreateFiller(propertyType, context));
        
    private IPropertyFiller CreateFiller(IPropertyType propertyType, PropertyFillerContext context)
    {
        return new RTEPropertyFiller(propertyType);
    }
}

public class RTEPropertyFiller(IPropertyType propertyType)
    : IReusablePropertyFiller
{
    public IPropertySink FillProperties(IPropertySink content, IGeneratorContext context)
    {
        var rng = context.GetRandom();
        FieldOptionsTextLipsum fieldOptions = new ()
        {
            Paragraphs = 1,
            Seed = rng.Next()
        };
        var loremipsumgenerator = RandomizerFactory.GetRandomizer(fieldOptions);

        var paragraphCount = rng.Next(1, 5);
        StringBuilder sb = new();
        for(int i = 0; i < paragraphCount; i++)
        {
            sb.Append("<p>");
            sb.Append(loremipsumgenerator.Generate());
            sb.Append("</p>");
        }
        content.SetValue(propertyType.Alias, sb.ToString(), null, null);

        return content;
    }

    public IPropertyFiller Reuse(IPropertyType propertyType)
    {
        return new RTEPropertyFiller(propertyType);
    }
}
