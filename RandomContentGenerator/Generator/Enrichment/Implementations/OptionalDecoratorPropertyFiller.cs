namespace RandomContentGenerator.Generator.Enrichment.Implementations;

public class OptionalDecoratorPropertyFiller(IPropertyFiller decoratee)
    : IPropertyFiller
{
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
