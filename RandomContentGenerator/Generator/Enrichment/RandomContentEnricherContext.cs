using Umbraco.Cms.Core.Models;

namespace RandomContentGenerator.Generator.Enrichment;

public record RandomContentEnricherContext(
    IContent Parent,
    IContentType ContentType
);