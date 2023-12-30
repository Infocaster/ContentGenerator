using Newtonsoft.Json.Linq;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Services;

namespace RandomContentGenerator.Generator.Enrichment.Implementations;

public class BlocklistPropertyFillerFactory(
    IDataTypeService dataTypeService,
    IContentTypeService contentTypeService,
    IJsonSerializer jsonSerializer)
        : PropertyFillerFactoryBase("Umbraco.BlockList", reuseFiller: false)
{
    protected override async ValueTask<IPropertyFiller> CreateFillerAsync(IPropertyType propertyType, PropertyFillerContext context)
    {
        var config = propertyType.ConfigurationAs<BlockListConfiguration>(dataTypeService);
        
        var min = Math.Max(
            config.ValidationLimit.Min ?? 0,
            1
        );
        var max = config.ValidationLimit.Max ?? min + 10;

        // The filler needs to be created before the blockfactories so that we can reuse it recursively
        var filler = new BlocklistPropertyFiller(propertyType, min, max, jsonSerializer);
        context.ReusableFillers.Add(propertyType.DataTypeId, filler);
        
        var blockFactories = new List<BlockFactory>(config.Blocks.Length);
        foreach (var block in config.Blocks) blockFactories.Add(await CreateFactoryForBlock(context, block));

        filler.BlockFactories = blockFactories;
        return filler;
    }

    private async ValueTask<BlockFactory> CreateFactoryForBlock(PropertyFillerContext context, BlockListConfiguration.BlockConfiguration blockConfig)
    {
        Guid contentTypeKey = blockConfig.ContentElementTypeKey;
        var contentFillers = await CreateFillersByTypeKey(contentTypeKey, context);

        Guid? settingsTypeKey = blockConfig.SettingsElementTypeKey;
        IReadOnlyCollection<IPropertyFiller>? settingsFillers = null;
        if (settingsTypeKey.HasValue)
        {
            settingsFillers = await CreateFillersByTypeKey(settingsTypeKey.Value, context);
        }

        return new BlockFactory(
            blockConfig.ContentElementTypeKey,
            contentFillers,
            settingsTypeKey,
            settingsFillers);
    }

    private ValueTask<IReadOnlyCollection<IPropertyFiller>> CreateFillersByTypeKey(Guid typeKey, PropertyFillerContext context)
    {
        var type = contentTypeService.Get(typeKey);
        if (type is null) throw new InvalidOperationException("Cannot create block factory because the content type is unknown");

        var elementContext = new PropertyFillerContext(context.Parent, type, context);
        var fillers = context.FillerCollection.GetPropertyFillersAsync(elementContext);

        return fillers;
    }
}

public class BlocklistPropertyFiller(IPropertyType propertyType, int min, int max, IJsonSerializer jsonSerializer)
        : IReusablePropertyFiller
{
    private IReadOnlyList<BlockFactory>? blockFactories;

    public virtual IReadOnlyList<BlockFactory> BlockFactories
    {
        get => blockFactories ?? throw new InvalidOperationException("Cannot use this filler before block factories are assigned");
        set => blockFactories = value;
    }

    public IPropertySink FillProperties(IPropertySink content, IGeneratorContext context)
    {
        // Early return to deal with potential infinite recursion
        // 5 is to be reasonable with optional properties
        // 20 is to hard-cap recursion to prevent an infinite loop 
        if (context.IsMaxRecursionReached(propertyType.Mandatory ? 20 : 5))
            return content;

        context.IncreaseRecursionLevel();

        Random rnd = context.GetRandom();
        var blockCount = rnd.Next(min, max);

        var layout = new List<object>();

        var blocks = new List<BlockItemData>(blockCount);
        var blockSettings = new List<BlockItemData>(blockCount);
        for (int i = 0; i < blockCount; i++)
        {
            var factory = BlockFactories[rnd.Next(0, BlockFactories.Count)];

            var block = factory.CreateBlock(context);
            var settings = factory.CreateSettings(context);
            blocks.Add(block);

            if (settings is not null) blockSettings.Add(settings);

            layout.Add(new { contentUdi = block.Udi, settingsUdi = settings?.Udi });
        }

        context.DecreaseRecursionLevel();

        var result = new BlockValue
        {
            Layout = new Dictionary<string, JToken>
            {
                ["Umbraco.BlockList"] = JToken.FromObject(layout)
            },
            ContentData = blocks,
            SettingsData = blockSettings
        };

        content.SetValue(propertyType.Alias, jsonSerializer.Serialize(result), null, null);

        return content;
    }

    public IPropertyFiller Reuse(IPropertyType propertyType)
    {
        return new ReusedBlocklistPropertyFiller(this, propertyType, min, max, jsonSerializer);
    }

    private class ReusedBlocklistPropertyFiller(BlocklistPropertyFiller original, IPropertyType propertyType, int min, int max, IJsonSerializer jsonSerializer)
        : BlocklistPropertyFiller(propertyType, min, max, jsonSerializer)
    {
        public override IReadOnlyList<BlockFactory> BlockFactories
        {
            get => original.BlockFactories;
            set => original.BlockFactories = value;
        }
    }
}