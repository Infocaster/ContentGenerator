using Umbraco.Cms.Core.Models;

namespace RandomContentGenerator.Generator.Production;

public record RandomContentFactoryContext(
    IContent Parent,
    IContentType TargetContentType,
    int UserId,
    IGeneratorContext GeneratorContext
);