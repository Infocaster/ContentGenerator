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

        // TODO: is there a better way to do this? min should always be at least 1.
        //       min is exactly one when multiple is false and at least 1 if multiple is true.
        var min = config.Multiple ? Math.Max(
            config.ValidationLimit.Min ?? 1,
            1
        ) : 1;
        var max = config.Multiple ? config.ValidationLimit.Max ?? min + 10 : 1;

        return new MediaPicker3PropertyFiller(propertyType, min, max, mediaService, rootId, filters, jsonSerializer, scopeProvider);
    }
}

public class MediaPicker3PropertyFiller(IPropertyType propertyType, int min, int max, IMediaService mediaService, int parentId, IDictionary<string, int>? filters, IJsonSerializer jsonSerializer, IScopeProvider scopeProvider)
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
        if (mediaCount < min) throw new InvalidOperationException("Cannot generate value for MNTP, because the minimum amount of documents exceeds the amount available");

        var amount = rnd.Next(min, Math.Min(max + 1, mediaCount));
        if (amount > 0)
        {
            var indexes = rnd.GetRandomInRange(amount, 0, mediaCount);
            var mediaItems = indexes
                .Select(i => mediaService.GetPagedDescendants(parentId, i, 1, out _, query).First())
                .Select(m => new MediaWithCropsDto
                {
                    Key = Guid.NewGuid(),
                    MediaKey = m.Key
                })
                .ToList();

            content.SetValue(propertyType.Alias, jsonSerializer.Serialize(mediaItems), null, null);
        }

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
