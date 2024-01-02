using Umbraco.Cms.Core.Composing;

namespace ContentGenerator.Generator.Enrichment;

public class FillerCollectionBuilder
    : OrderedCollectionBuilderBase<FillerCollectionBuilder, FillerCollection, IPropertyFillerFactory>
{
    protected override FillerCollectionBuilder This => this;
}

public interface IFillerCollection
{
    ValueTask<IReadOnlyCollection<IPropertyFiller>> GetPropertyFillersAsync(PropertyFillerContext context);
}

public class FillerCollection : BuilderCollectionBase<IPropertyFillerFactory>
    , IFillerCollection
{
    public FillerCollection(Func<IEnumerable<IPropertyFillerFactory>> items) : base(items)
    {
    }

    public async ValueTask<IReadOnlyCollection<IPropertyFiller>> GetPropertyFillersAsync(PropertyFillerContext context)
    {
        List<IPropertyFiller> result = new ();

        foreach(var factory in this)
        {
            var fillerSet = await factory.CreateAsync(context);
            result.AddRange(fillerSet);
        }
        return result;
    }
}