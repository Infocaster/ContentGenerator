using Umbraco.Cms.Core.Models;

namespace ContentGenerator.Generator.Enrichment;

public record RandomContentEnricherContext(
    IContent Parent,
    IContentType ContentType
);