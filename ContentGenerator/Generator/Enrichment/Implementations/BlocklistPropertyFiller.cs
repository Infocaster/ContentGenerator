/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
using Newtonsoft.Json.Linq;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Services;

namespace ContentGenerator.Generator.Enrichment.Implementations;

public class BlocklistPropertyFillerFactory : PropertyFillerFactoryBase
{
    private readonly IDataTypeService dataTypeService;
    private readonly IContentTypeService contentTypeService;
    private readonly IJsonSerializer jsonSerializer;

    public BlocklistPropertyFillerFactory(
        IDataTypeService dataTypeService,
        IContentTypeService contentTypeService,
        IJsonSerializer jsonSerializer) : base("Umbraco.BlockList", reuseFiller: false)
    {
        this.dataTypeService = dataTypeService;
        this.contentTypeService = contentTypeService;
        this.jsonSerializer = jsonSerializer;
    }

    protected override async ValueTask<IPropertyFiller> CreateFillerAsync(IPropertyType propertyType, PropertyFillerContext context)
    {
        var config = propertyType.ConfigurationAs<BlockListConfiguration>(dataTypeService);
        
        var min = Math.Max(
            config.ValidationLimit.Min ?? 0,
            1
        );
        var max = config.ValidationLimit.Max ?? min + 10;

        // The filler needs to be created before the blockfactories so that we can reuse it recursively
        var filler = new BlocklistPropertyFiller(propertyType, min..(max + 1), jsonSerializer);
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

public class BlocklistPropertyFiller : IReusablePropertyFiller
{
    private readonly IPropertyType propertyType;
    private readonly Range sizeRange;
    private readonly IJsonSerializer jsonSerializer;
    private IReadOnlyList<BlockFactory>? blockFactories;

    public BlocklistPropertyFiller(IPropertyType propertyType, Range sizeRange, IJsonSerializer jsonSerializer)
    {
        this.propertyType = propertyType;
        this.sizeRange = sizeRange;
        this.jsonSerializer = jsonSerializer;
    }

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
        var blockCount = rnd.Next(sizeRange.Start.Value, sizeRange.End.Value);

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
        return new ReusedBlocklistPropertyFiller(this, propertyType, sizeRange, jsonSerializer);
    }

    private class ReusedBlocklistPropertyFiller : BlocklistPropertyFiller
    {
        private readonly BlocklistPropertyFiller original;

        public ReusedBlocklistPropertyFiller(BlocklistPropertyFiller original, IPropertyType propertyType, Range sizeRange, IJsonSerializer jsonSerializer) : base(propertyType, sizeRange, jsonSerializer)
        {
            this.original = original;
        }

        public override IReadOnlyList<BlockFactory> BlockFactories
        {
            get => original.BlockFactories;
            set => original.BlockFactories = value;
        }
    }
}