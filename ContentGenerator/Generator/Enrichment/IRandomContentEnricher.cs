using Umbraco.Cms.Core.Models;

namespace ContentGenerator.Generator.Enrichment;

public interface IRandomContentEnricherFactory
{
    ValueTask<IEnumerable<IRandomContentEnricher>> CreateAsync(ContentEnricherContext context);
}

public interface IRandomContentEnricher
{
    IContent Enrich(IContent content, IGeneratorContext context);
}

public class RandomContentEnricherFactory(IFillerCollection propertyFillerFactory)
        : IRandomContentEnricherFactory
{
    public async ValueTask<IEnumerable<IRandomContentEnricher>> CreateAsync(ContentEnricherContext context)
    {
        var fillerContext = new PropertyFillerContext(context.Parent, context.ContentType, propertyFillerFactory);

        var fillers = await propertyFillerFactory.GetPropertyFillersAsync(fillerContext);
        return new List<IRandomContentEnricher>{

            new PropertyFillerContentEnricher(fillers)
        };
    }
}

public class PropertyFillerContentEnricher(IReadOnlyCollection<IPropertyFiller> fillers)
        : IRandomContentEnricher
{
    public IContent Enrich(IContent content, IGeneratorContext context)
    {
        IPropertySink sink = new ContentPropertySink(content);
        foreach(var filler in fillers) sink = filler.FillProperties(sink, context);

        return content;
    }
}
