namespace RandomContentGenerator.Generator.Enrichment;

public interface IPropertyFiller
{
    IPropertySink FillProperties(IPropertySink content, IGeneratorContext context);
}

public interface IPropertyFillerFactory
{
    ValueTask<IReadOnlyCollection<IPropertyFiller>> CreateAsync(PropertyFillerContext context);
}