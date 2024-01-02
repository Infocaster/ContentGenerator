using ContentGenerator.Generator;
using ContentGenerator.Generator.Enrichment;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace ContentGenerator.Website.RandomContentExtensions;

public class YoutubeUrlPropertyFillerFactory
    : IPropertyFillerFactory
{
    public ValueTask<IReadOnlyCollection<IPropertyFiller>> CreateAsync(PropertyFillerContext context)
        => ValueTask.FromResult(Create(context));
        
    public IReadOnlyCollection<IPropertyFiller> Create(PropertyFillerContext context)
    {
        var result = new List<IPropertyFiller>();

        if (context.ContentType.Alias == VideoRow.ModelTypeAlias)
        {
            var videoUrlProperty = context.Properties.First(p => p.Alias == "videoUrl");
            IPropertyFiller item = new YoutubeUrlPropertyFiller(videoUrlProperty);
            if (!videoUrlProperty.Mandatory) item = item.MakeOptional();

            result.Add(item);
            context.Consume(videoUrlProperty);
        }

        return result;
    }
}

public class YoutubeUrlPropertyFiller : IPropertyFiller
{
    private readonly IPropertyType propertyType;

    public YoutubeUrlPropertyFiller(IPropertyType propertyType)
    {
        this.propertyType = propertyType;
    }

    public IPropertySink FillProperties(IPropertySink content, IGeneratorContext context)
    {
        content.SetValue(propertyType.Alias, "https://youtu.be/KaFtsqU2V6U?si=982_Mjbbc3l8NPj6", null, null);
        return content;
    }
}