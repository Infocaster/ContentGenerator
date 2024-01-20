/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
using System.Text;
using RandomDataGenerator.FieldOptions;
using RandomDataGenerator.Randomizers;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;

namespace ContentGenerator.Generator.Enrichment.Implementations;

public class TextAreaPropertyFillerFactory : PropertyFillerFactoryBase
{
    private readonly IDataTypeService dataTypeService;

    public TextAreaPropertyFillerFactory(IDataTypeService dataTypeService) : base("Umbraco.TextArea")
    {
        this.dataTypeService = dataTypeService;
    }

    protected override ValueTask<IPropertyFiller> CreateFillerAsync(IPropertyType propertyType, PropertyFillerContext context)
        => ValueTask.FromResult(CreateFiller(propertyType, context));
        
    private IPropertyFiller CreateFiller(IPropertyType propertyType, PropertyFillerContext context)
    {
        var config = propertyType.ConfigurationAs<TextAreaConfiguration>(dataTypeService);

        var max = config.MaxChars ?? 1000;

        return new TextAreaPropertyFiller(propertyType, max);
    }
}

public class TextAreaPropertyFiller : IReusablePropertyFiller
{
    private readonly IPropertyType propertyType;
    private readonly int max;

    public TextAreaPropertyFiller(IPropertyType propertyType, int max)
    {
        this.propertyType = propertyType;
        this.max = max;
    }

    public IPropertySink FillProperties(IPropertySink content, IGeneratorContext context)
    {
        var rnd = context.GetRandom();

        var targetCharacterCount = rnd.Next(1, max + 1);

        var loremipsumrandomizer = RandomizerFactory.GetRandomizer(new FieldOptionsTextLipsum() { Seed = rnd.Next(), Paragraphs = 1 });
        StringBuilder sb = new (max);
        while (sb.Length < targetCharacterCount)
        {
            sb.AppendLine(loremipsumrandomizer.Generate());
        }

        sb.Length = targetCharacterCount;
        content.SetValue(propertyType.Alias, sb.ToString().Trim(), null, null);

        return content;
    }

    public IPropertyFiller Reuse(IPropertyType propertyType)
    {
        return new TextAreaPropertyFiller(propertyType, max);
    }
}
