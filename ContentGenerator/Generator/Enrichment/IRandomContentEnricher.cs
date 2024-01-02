using Umbraco.Cms.Core.Models;

namespace ContentGenerator.Generator.Enrichment;

public interface IRandomContentEnricherFactory
{
    ValueTask<IEnumerable<IRandomContentEnricher>> CreateAsync(RandomContentEnricherContext context);
}

public interface IRandomContentEnricher
{
    IContent Enrich(IContent content, IGeneratorContext context);
}

public class RandomContentEnricherFactory : IRandomContentEnricherFactory
{
    private readonly IFillerCollection propertyFillerFactory;

    public RandomContentEnricherFactory(IFillerCollection propertyFillerFactory)
    {
        this.propertyFillerFactory = propertyFillerFactory;
    }

    public async ValueTask<IEnumerable<IRandomContentEnricher>> CreateAsync(RandomContentEnricherContext context)
    {
        var fillerContext = new PropertyFillerContext(context.Parent, context.ContentType, propertyFillerFactory);

        var fillers = await propertyFillerFactory.GetPropertyFillersAsync(fillerContext);
        return new List<IRandomContentEnricher>{

            new PropertyFillerContentEnricher(fillers)
        };
    }
}

public class PropertyFillerContentEnricher : IRandomContentEnricher
{
    private readonly IReadOnlyCollection<IPropertyFiller> fillers;

    public PropertyFillerContentEnricher(IReadOnlyCollection<IPropertyFiller> fillers)
    {
        this.fillers = fillers;
    }

    public IContent Enrich(IContent content, IGeneratorContext context)
    {
        IPropertySink sink = new ContentPropertySink(content);
        foreach(var filler in fillers) sink = filler.FillProperties(sink, context);

        return content;
    }
}
