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

public class TextStringPropertyFillerFactory : PropertyFillerFactoryBase
{
    private readonly IDataTypeService dataTypeService;

    public TextStringPropertyFillerFactory(IDataTypeService dataTypeService) : base("Umbraco.TextBox")
    {
        this.dataTypeService = dataTypeService;
    }

    protected override ValueTask<IPropertyFiller> CreateFillerAsync(IPropertyType propertyType, PropertyFillerContext context)
        => ValueTask.FromResult(CreateFiller(propertyType, context));
        
    private IPropertyFiller CreateFiller(IPropertyType propertyType, PropertyFillerContext context)
    {
        var config = propertyType.ConfigurationAs<TextboxConfiguration>(dataTypeService);

        var max = config.MaxChars ?? 400;

        return new TextStringPropertyFiller(propertyType, max);
    }
}

public class TextStringPropertyFiller : IReusablePropertyFiller
{
    private readonly IPropertyType propertyType;
    private readonly int max;

    public TextStringPropertyFiller(IPropertyType propertyType, int max)
    {
        this.propertyType = propertyType;
        this.max = max;
    }

    public IPropertySink FillProperties(IPropertySink content, IGeneratorContext context)
    {
        var rnd = context.GetRandom();

        var targetCharacterCount = rnd.Next(1, max + 1);

        var loremipsumrandomizer = RandomizerFactory.GetRandomizer(new FieldOptionsTextWords() { Seed = rnd.Next(), Min = 1, Max = 1 });
        StringBuilder sb = new (max);
        while (sb.Length < targetCharacterCount)
        {
            sb.Append(loremipsumrandomizer.Generate());
            sb.Append(' ');
        }

        sb.Length = targetCharacterCount;

        content.SetValue(propertyType.Alias, sb.ToString().Trim(), null, null);
        return content;
    }

    public IPropertyFiller Reuse(IPropertyType propertyType)
    {
        return new TextStringPropertyFiller(propertyType, max);
    }
}