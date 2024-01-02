using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models.Blocks;

namespace ContentGenerator.Generator.Enrichment.Implementations;

public class BlockFactory(
    Guid contentTypeKey,
    IReadOnlyCollection<IPropertyFiller> contentPropertyFillers,
    Guid? settingsTypeKey,
    IReadOnlyCollection<IPropertyFiller>? settingsPropertyFillers)
{
    public BlockItemData CreateBlock(IGeneratorContext context)
    {
        BlockItemData result = CreateModel(contentTypeKey);

        var sink = new DictionaryPropertySink(result.RawPropertyValues);
        foreach (var filler in contentPropertyFillers) filler.FillProperties(sink, context);

        return result;
    }

    public BlockItemData? CreateSettings(IGeneratorContext context)
    {
        if (settingsTypeKey is null) return null;

        BlockItemData result = CreateModel(settingsTypeKey.Value);

        var sink = new DictionaryPropertySink(result.RawPropertyValues);
        foreach (var filler in settingsPropertyFillers!) filler.FillProperties(sink, context);

        return result;
    }

    private BlockItemData CreateModel(Guid elementTypeKey)
    {
        return new BlockItemData
        {
            ContentTypeKey = elementTypeKey,
            Udi = Udi.Create(Constants.UdiEntityType.Element, Guid.NewGuid())
        };
    }
}