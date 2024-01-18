using Umbraco.Cms.Core.Models;

namespace ContentGenerator.Generator.Production;

public record ContentFactoryContext(
    IContent Parent,
    IContentType TargetContentType,
    int UserId,
    IGeneratorContext GeneratorContext
);