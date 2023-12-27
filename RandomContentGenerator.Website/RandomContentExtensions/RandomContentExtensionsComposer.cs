using RandomContentGenerator.Generator.Enrichment;
using Umbraco.Cms.Core.Composing;

namespace RandomContentGenerator.Website.RandomContentExtensions;

public class RandomContentExtensionsComposer
    : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.WithCollectionBuilder<FillerCollectionBuilder>()
            .Insert<YoutubeUrlPropertyFillerFactory>(0);
    }
}
