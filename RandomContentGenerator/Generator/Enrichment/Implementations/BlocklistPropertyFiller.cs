using Newtonsoft.Json.Linq;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Services;
using Umbraco.Extensions;

namespace RandomContentGenerator.Generator.Enrichment.Implementations;

public class BlocklistPropertyFillerFactory(
    IDataTypeService dataTypeService,
    IContentTypeService contentTypeService,
    IJsonSerializer jsonSerializer)
        : PropertyFillerFactoryBase("Umbraco.BlockList")
{
    protected override async ValueTask<IPropertyFiller> CreateFillerAsync(IPropertyType propertyType, PropertyFillerContext context)
    {
        var config = propertyType.ConfigurationAs<BlockListConfiguration>(dataTypeService);

        var blockFactories = new List<BlockFactory>(config.Blocks.Length);
        foreach(var block in config.Blocks) blockFactories.Add(await CreateFactoryForBlock(context, block));

        return new BlocklistPropertyFiller(propertyType, blockFactories, jsonSerializer);
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

        var elementContext = new PropertyFillerContext(context.Parent, type, context.FillerCollection);
        var fillers = context.FillerCollection.GetPropertyFillersAsync(elementContext);

        return fillers;
    }
}

public class BlocklistPropertyFiller(IPropertyType propertyType, IReadOnlyList<BlockFactory> blockFactories, IJsonSerializer jsonSerializer)
        : IPropertyFiller
{
    public IPropertySink FillProperties(IPropertySink content, IGeneratorContext context)
    {
        Random rnd = context.GetRandom();
        var blockCount = rnd.Next(1, 10);

        var layout = new List<object>();

        var blocks = new List<BlockItemData>(blockCount);
        var blockSettings = new List<BlockItemData>(blockCount);
        for (int i = 0; i < blockCount; i++)
        {
            var factory = blockFactories[rnd.Next(0, blockFactories.Count)];

            var block = factory.CreateBlock(context);
            var settings = factory.CreateSettings(context);
            blocks.Add(block);

            if (settings is not null) blockSettings.Add(settings);

            layout.Add(new { contentUdi = block.Udi, settingsUdi = settings?.Udi });
        }

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
}