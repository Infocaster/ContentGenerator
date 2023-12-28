using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.DynamicRoot;
using Umbraco.Cms.Core.DynamicRoot.QuerySteps;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.Entities;
using Umbraco.Cms.Core.Persistence.Querying;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Xml;
using Umbraco.Cms.Infrastructure.Scoping;
using Umbraco.Extensions;

namespace RandomContentGenerator.Generator.Enrichment.Implementations;

public class MultiNodeTreePickerPropertyFillerFactory(
    IDataTypeService dataTypeService,
    IContentService contentService,
    IContentTypeService contentTypeService,
    IEntityService entityService,
    IServiceScopeFactory serviceScopeFactory,
    IDynamicRootService dynamicRootService,
    IJsonSerializer jsonSerializer,
    IScopeProvider scopeProvider)
    : PropertyFillerFactoryBase("Umbraco.MultiNodeTreePicker")
{
    protected override async ValueTask<IPropertyFiller> CreateFillerAsync(IPropertyType propertyType, PropertyFillerContext context)
    {
        var config = propertyType.ConfigurationAs<MultiNodePickerConfiguration>(dataTypeService);
        
        using var serviceScope = serviceScopeFactory.CreateAsyncScope();

        int rootId = await GetRootAsync(contentService, serviceScope.ServiceProvider.GetRequiredService<IPublishedContentQuery>(), dynamicRootService, context, config);

        IDictionary<string, int>? filters = null;
        if (!string.IsNullOrWhiteSpace(config.Filter))
        {
            var keys = config.Filter.Split(',').Select(s => s.Trim()).ToList();
            filters = keys.ToDictionary(k => k, k => contentTypeService.Get(k)!.Id);
        }

        return new MultiNodeTreePickerPropertyFiller(propertyType, contentService, rootId, filters, jsonSerializer, scopeProvider);
    }

    private async Task<int> GetRootAsync(IContentService contentService, IPublishedContentQuery publishedContentQuery, IDynamicRootService dynamicRootService, PropertyFillerContext context, MultiNodePickerConfiguration config)
    {
        int rootId = Constants.System.Root;
        if (config.TreeSource?.StartNodeId is not null)
        {
            var rootContent = contentService.GetById(Guid.Parse(config.TreeSource.StartNodeId.ToString()[^32..]));
            if (rootContent is not null) rootId = rootContent.Id;
        }
        else if (!string.IsNullOrEmpty(config.TreeSource?.StartNodeQuery))
        {
            var xpathQuery = ParseXPathQuery(config.TreeSource.StartNodeQuery, 0, context.Parent.Id, publishedContentQuery);
            var rootContent = publishedContentQuery.ContentSingleAtXPath(xpathQuery);
            if (rootContent is not null) rootId = rootContent.Id;
        }
        else if (config.TreeSource?.DynamicRoot is not null)
        {
            var query = new DynamicRootNodeQuery
            {
                Context = new DynamicRootContext
                {
                    CurrentKey = null,
                    ParentKey = context.Parent.Key
                },
                OriginKey = config.TreeSource.DynamicRoot.OriginKey,
                OriginAlias = config.TreeSource.DynamicRoot.OriginAlias,
                QuerySteps = config.TreeSource.DynamicRoot.QuerySteps.Select(x => new DynamicRootQueryStep
                {
                    Alias = x.Alias,
                    AnyOfDocTypeKeys = x.AnyOfDocTypeKeys
                })
            };

            var startNodes = (await dynamicRootService.GetDynamicRootsAsync(query)).ToList();

            if (startNodes.Count > 0)
            {
                var rootContent = contentService.GetById(startNodes.First());
                if (rootContent is not null) rootId = rootContent.Id;
            }
        }

        return rootId;
    }

    private string ParseXPathQuery(string query, int id, int? parentId, IPublishedContentQuery publishedContentQuery) =>
        UmbracoXPathPathSyntaxParser.ParseXPathQuery(
            query,
            id,
            parentId,
            nodeid =>
            {
                IEntitySlim? ent = entityService.Get(nodeid);
                return ent?.Path.Split(Constants.CharArrays.Comma).Reverse();
            },
            i => publishedContentQuery.Content(i) != null);
}

public class MultiNodeTreePickerPropertyFiller(IPropertyType propertyType, IContentService contentService, int parentId, IDictionary<string, int>? filters, IJsonSerializer jsonSerializer, IScopeProvider scopeProvider)
        : IPropertyFiller
{
    public IPropertySink FillProperties(IPropertySink content, IGeneratorContext context)
    {
        Random rnd = context.GetRandom();

        KeyValuePair<string, int>? filter = null;
        if (filters is not null)
        {
            filter = filters.ElementAt(rnd.Next(filters.Count));
        }

        using var scope = scopeProvider.CreateScope();

        IQuery<IContent>? query = null;
        if (filter is not null)
        {
            query = scope.SqlContext.Query<IContent>().Where(m => m.ContentTypeId == filter.Value.Value);
        }

        var contentCount = contentService.CountDescendants(parentId, filter?.Key);
        int pageIndex = rnd.Next(0, contentCount);
        var randomContentItem = contentService.GetPagedDescendants(parentId, pageIndex, 1, out _, query).First();

        content.SetValue(propertyType.Alias, Udi.Create(Constants.UdiEntityType.Document, randomContentItem.Key).ToString(), null, null);

        scope.Complete();

        return content;
    }
}
