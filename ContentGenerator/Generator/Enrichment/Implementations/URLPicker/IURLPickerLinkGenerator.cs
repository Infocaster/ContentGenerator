using RandomDataGenerator.FieldOptions;
using RandomDataGenerator.Randomizers;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;

namespace ContentGenerator.Generator.Enrichment.Implementations.URLPicker;

public interface IURLPickerLinkGenerator
{
    MultiUrlPickerValueEditor.LinkDto GenerateLink(IGeneratorContext context);
}

public abstract class URLPickerLinkGeneratorBase
    : IURLPickerLinkGenerator
{
    public virtual MultiUrlPickerValueEditor.LinkDto GenerateLink(IGeneratorContext context)
    {
        var rnd = context.GetRandom();

        var loremipsumrandomizer = RandomizerFactory.GetRandomizer(new FieldOptionsTextWords() { Seed = context.GetRandom().Next() });

        return new MultiUrlPickerValueEditor.LinkDto
        {
            Name = loremipsumrandomizer.Generate(),
            Target = rnd.NextDouble() < 0.5 ? "_blank" : null
        };
    }
}

public class URLPickerByMediaLinkGenerator : URLPickerLinkGeneratorBase
{
    private readonly IMediaService mediaService;

    public URLPickerByMediaLinkGenerator(IMediaService mediaService)
    {
        this.mediaService = mediaService;
    }

    public override MultiUrlPickerValueEditor.LinkDto GenerateLink(IGeneratorContext context)
    {
        var rnd = context.GetRandom();

        var contentCount = mediaService.CountDescendants(Constants.System.Root);
        var randomContent = mediaService.GetPagedDescendants(Constants.System.Root, rnd.Next(0, contentCount), 1, out _).First();

        var result = base.GenerateLink(context);
        result.Udi = new GuidUdi(Constants.UdiEntityType.Media, randomContent.Key);
        return result;
    }
}

public class URLPickerByContentLinkGenerator : URLPickerLinkGeneratorBase
{
    private readonly IContentService contentService;

    public URLPickerByContentLinkGenerator(IContentService contentService)
    {
        this.contentService = contentService;
    }

    public override MultiUrlPickerValueEditor.LinkDto GenerateLink(IGeneratorContext context)
    {
        var rnd = context.GetRandom();

        var contentCount = contentService.CountDescendants(Constants.System.Root);
        var randomContent = contentService.GetPagedDescendants(Constants.System.Root, rnd.Next(0, contentCount), 1, out _).First();

        var result = base.GenerateLink(context);
        result.Udi = new GuidUdi(Constants.UdiEntityType.Document, randomContent.Key);
        return result;
    }
}

public class URLPickerByTextLinkGenerator
    : URLPickerLinkGeneratorBase
{
    public override MultiUrlPickerValueEditor.LinkDto GenerateLink(IGeneratorContext context)
    {
        var rnd = context.GetRandom();

        var randomUrlGenerator = RandomizerFactory.GetRandomizer(new FieldOptionsTextRegex { Pattern = @"^https?://\w+(\.\w)*\.(com|net|info|org)(/\w+)*$", Seed = rnd.Next() });

        var result = base.GenerateLink(context);
        result.Url = randomUrlGenerator.Generate();
        return result;
    }
}