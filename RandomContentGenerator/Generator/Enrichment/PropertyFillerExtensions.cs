using RandomContentGenerator.Generator.Enrichment.Implementations;

namespace RandomContentGenerator.Generator.Enrichment;

public static class PropertyFillerExtensions
{
    public static IPropertyFiller MakeOptional(this IPropertyFiller source)
        => new OptionalDecoratorPropertyFiller(source);
}