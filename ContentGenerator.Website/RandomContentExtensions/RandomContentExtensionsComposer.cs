using ContentGenerator.Generator.Enrichment;
using Umbraco.Cms.Core.Composing;

namespace ContentGenerator.Website.RandomContentExtensions;

public class RandomContentExtensionsComposer
    : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.WithCollectionBuilder<FillerCollectionBuilder>()
            .Insert<YoutubeUrlPropertyFillerFactory>(0);
    }
}
