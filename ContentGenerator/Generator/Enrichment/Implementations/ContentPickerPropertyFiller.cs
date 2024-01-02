
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;
using Umbraco.Extensions;

namespace ContentGenerator.Generator.Enrichment.Implementations;

public class ContentPickerPropertyFillerFactory : PropertyFillerFactoryBase
{
    private readonly IDataTypeService dataTypeService;
    private readonly IContentService contentService;

    public ContentPickerPropertyFillerFactory(
        IDataTypeService dataTypeService,
        IContentService contentService) : base("Umbraco.ContentPicker")
    {
        this.dataTypeService = dataTypeService;
        this.contentService = contentService;
    }

    protected override ValueTask<IPropertyFiller> CreateFillerAsync(IPropertyType propertyType, PropertyFillerContext context)
        => ValueTask.FromResult(CreateFiller(propertyType, context));
        
    private IPropertyFiller CreateFiller(IPropertyType propertyType, PropertyFillerContext context)
    {
        var config = propertyType.ConfigurationAs<ContentPickerConfiguration>(dataTypeService);

        IContent? startNode = null;
        if (config.StartNodeId is not null) startNode = contentService.GetById(Guid.Parse(config.StartNodeId.ToString()[^32..]));
        
        var rootId = startNode?.Id ?? Constants.System.Root;
        
        return new ContentPickerPropertyFiller(propertyType, contentService, rootId);
    }
}

public class ContentPickerPropertyFiller : IReusablePropertyFiller
{
    private readonly IPropertyType propertyType;
    private readonly IContentService contentService;
    private readonly int parentId;

    public ContentPickerPropertyFiller(IPropertyType propertyType, IContentService contentService, int parentId)
    {
        this.propertyType = propertyType;
        this.contentService = contentService;
        this.parentId = parentId;
    }

    public IPropertySink FillProperties(IPropertySink content, IGeneratorContext context)
    {
        Random rnd = context.GetRandom();

        var descendantCount = contentService.CountDescendants(parentId);
        var randomContentItem = contentService.GetPagedDescendants(parentId, rnd.Next(0, descendantCount), 1, out _).First();

        var value = "umb://document/" + randomContentItem.Key.ToString("N");
        content.SetValue(propertyType.Alias, value, null, null);

        return content;
    }

    public IPropertyFiller Reuse(IPropertyType propertyType)
    {
        return new ContentPickerPropertyFiller(propertyType, contentService, parentId);
    }
}
