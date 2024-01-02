namespace ContentGenerator.Generator.Enrichment.Implementations;

public class OptionalDecoratorPropertyFiller : IPropertyFiller
{
    private readonly IPropertyFiller decoratee;

    public OptionalDecoratorPropertyFiller(IPropertyFiller decoratee)
    {
        this.decoratee = decoratee;
    }

    public IPropertySink FillProperties(IPropertySink content, IGeneratorContext context)
    {
        var ratio = context.GetOptionalRatio();

        Random rnd = context.GetRandom();
        if (rnd.NextDouble() < ratio)
        {
            content = decoratee.FillProperties(content, context);
        }

        return content;
    }
}
