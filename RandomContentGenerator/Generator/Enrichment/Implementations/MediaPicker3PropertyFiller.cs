using System.Runtime.Serialization;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Persistence.Querying;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.PropertyEditors.ValueConverters;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Scoping;
using Umbraco.Extensions;

namespace RandomContentGenerator.Generator.Enrichment.Implementations;

public class MediaPicker3PropertyFillerFactory(IMediaService mediaService, IMediaTypeService mediaTypeService, IDataTypeService dataTypeService, IJsonSerializer jsonSerializer, IScopeProvider scopeProvider)
        : PropertyFillerFactoryBase("Umbraco.MediaPicker3")
{
    protected override ValueTask<IPropertyFiller> CreateFillerAsync(IPropertyType propertyType, PropertyFillerContext context)
        => ValueTask.FromResult(CreateFiller(propertyType, context));
        
    private IPropertyFiller CreateFiller(IPropertyType propertyType, PropertyFillerContext context)
    {
        var config = propertyType.ConfigurationAs<MediaPicker3Configuration>(dataTypeService);

        IMedia? startNode = null;
        if (config.StartNodeId is not null) startNode = mediaService.GetById(Guid.Parse(config.StartNodeId.ToString()[^32..]));

        IDictionary<string, int>? filters = null;
        if (!string.IsNullOrWhiteSpace(config.Filter))
        {
            var keys = config.Filter.Split(',').Select(s => s.Trim()).ToList();
            filters = keys.ToDictionary(k => k, k => mediaTypeService.Get(k)!.Id);
        }

        var rootId = startNode?.Id ?? Constants.System.Root;
        return new MediaPicker3PropertyFiller(propertyType, mediaService, rootId, filters, jsonSerializer, scopeProvider);
    }
}

public class MediaPicker3PropertyFiller(IPropertyType propertyType, IMediaService mediaService, int parentId, IDictionary<string, int>? filters, IJsonSerializer jsonSerializer, IScopeProvider scopeProvider)
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

        IQuery<IMedia>? query = null;
        if (filter is not null)
        {
            query = scope.SqlContext.Query<IMedia>().Where(m => m.ContentTypeId == filter.Value.Value);
        }

        var mediaCount = mediaService.CountDescendants(parentId, filter?.Key);
        var randomMediaItem = mediaService.GetPagedDescendants(parentId, rnd.Next(0, mediaCount), 1, out _, query).First();

        var value = new[]
        {
            new MediaWithCropsDto
            {
                Key = Guid.NewGuid(),
                MediaKey = randomMediaItem.Key
            }
        };

        content.SetValue(propertyType.Alias, jsonSerializer.Serialize(value), null, null);

        scope.Complete();

        return content;
    }

    [DataContract]
    internal class MediaWithCropsDto
    {
        [DataMember(Name = "key")]
        public Guid Key { get; set; }

        [DataMember(Name = "mediaKey")]
        public Guid MediaKey { get; set; }

        [DataMember(Name = "crops")]
        public IEnumerable<ImageCropperValue.ImageCropperCrop>? Crops { get; set; }

        [DataMember(Name = "focalPoint")]
        public ImageCropperValue.ImageCropperFocalPoint? FocalPoint { get; set; }
    }
}
