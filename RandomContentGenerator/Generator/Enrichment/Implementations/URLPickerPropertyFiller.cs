using RandomDataGenerator.FieldOptions;
using RandomDataGenerator.Randomizers;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Services;

namespace RandomContentGenerator.Generator.Enrichment.Implementations;

public class URLPickerPropertyFillerFactory(IDataTypeService dataTypeService, IContentService contentService, IMediaService mediaService, IJsonSerializer jsonSerializer)
    : PropertyFillerFactoryBase("Umbraco.MultiUrlPicker")
{
    protected override ValueTask<IPropertyFiller> CreateFillerAsync(IPropertyType propertyType, PropertyFillerContext context)
        => ValueTask.FromResult(CreateFiller(propertyType, context));

    private IPropertyFiller CreateFiller(IPropertyType propertyType, PropertyFillerContext context)
    {
        var config = propertyType.ConfigurationAs<MultiUrlPickerConfiguration>(dataTypeService);

        // Since mandatory properties cannot contain 0 items,
        //    the minimum for this property is always at least 1
        var min = Math.Max(config.MinNumber, 1);

        var max = config.MaxNumber;
        if (max == default) max = min + 10;
        
        return new URLPickerPropertyFiller(propertyType, min, max, contentService, mediaService, jsonSerializer);
    }
}

public class URLPickerPropertyFiller(IPropertyType propertyType, int min, int max, IContentService contentService, IMediaService mediaService, IJsonSerializer jsonSerializer)
    : IReusablePropertyFiller
{
    private delegate MultiUrlPickerValueEditor.LinkDto LinkGenerator(IGeneratorContext context);

    public IPropertySink FillProperties(IPropertySink content, IGeneratorContext context)
    {
        LinkGenerator[] generators = [
            LinkByUrl,
            LinkByContent,
            LinkByMedia
        ];

        var rnd = context.GetRandom();
        
        var amount = rnd.Next(min, max + 1);

        List<MultiUrlPickerValueEditor.LinkDto> result = new (amount);

        for (int i = 0; i < amount; i++)
        {
            var generator = generators[rnd.Next(generators.Length)];
            result.Add(generator(context));
        }

        content.SetValue(propertyType.Alias, jsonSerializer.Serialize(result), null, null);
        return content;
    }

    private MultiUrlPickerValueEditor.LinkDto LinkByUrl(IGeneratorContext context)
    {
        var rnd = context.GetRandom();

        var randomUrlGenerator = RandomizerFactory.GetRandomizer(new FieldOptionsTextRegex { Pattern = @"^https?://\w+(\.\w)*\.(com|net|info|org)(/\w+)*$", Seed = rnd.Next() });

        var result = GetLinkBase(context);
        result.Url = randomUrlGenerator.Generate();
        return result;
    }

    private MultiUrlPickerValueEditor.LinkDto LinkByContent(IGeneratorContext context)
    {
        var rnd = context.GetRandom();

        var contentCount = contentService.CountDescendants(Constants.System.Root);
        var randomContent = contentService.GetPagedDescendants(Constants.System.Root, rnd.Next(0, contentCount), 1, out _).First();

        var result = GetLinkBase(context);
        result.Udi = new GuidUdi(Constants.UdiEntityType.Document, randomContent.Key);
        return result;
    }

    private MultiUrlPickerValueEditor.LinkDto LinkByMedia(IGeneratorContext context)
    {
        var rnd = context.GetRandom();

        var contentCount = mediaService.CountDescendants(Constants.System.Root);
        var randomContent = mediaService.GetPagedDescendants(Constants.System.Root, rnd.Next(0, contentCount), 1, out _).First();

        var result = GetLinkBase(context);
        result.Udi = new GuidUdi(Constants.UdiEntityType.Media, randomContent.Key);
        return result;
    }

    private MultiUrlPickerValueEditor.LinkDto GetLinkBase(IGeneratorContext context)
    {
        var rnd = context.GetRandom();

        var loremipsumrandomizer = RandomizerFactory.GetRandomizer(new FieldOptionsTextWords() { Seed = context.GetRandom().Next() });

        return new MultiUrlPickerValueEditor.LinkDto
        {
            Name = loremipsumrandomizer.Generate(),
            Target = rnd.NextDouble() < 0.5 ? "_blank" : null
        };
    }

    public IPropertyFiller Reuse(IPropertyType propertyType)
    {
        return new URLPickerPropertyFiller(propertyType, min, max, contentService, mediaService, jsonSerializer);
    }
}
