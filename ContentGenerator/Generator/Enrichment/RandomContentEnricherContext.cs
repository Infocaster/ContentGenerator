using Umbraco.Cms.Core.Models;

namespace ContentGenerator.Generator.Enrichment;

public record ContentEnricherContext(
    IContent Parent,
    IContentType ContentType
);