/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;
using Umbraco.Extensions;

namespace ContentGenerator.Generator.Enrichment.Implementations;

public class ContentPickerPropertyFillerFactory(
    IDataTypeService dataTypeService,
    IContentService contentService)
        : PropertyFillerFactoryBase("Umbraco.ContentPicker")
{
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

public class ContentPickerPropertyFiller(IPropertyType propertyType, IContentService contentService, int parentId)
        : IReusablePropertyFiller
{
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
