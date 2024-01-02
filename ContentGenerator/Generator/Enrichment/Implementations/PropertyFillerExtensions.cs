using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Extensions;

namespace ContentGenerator.Generator.Enrichment.Implementations;

public static class PropertyFillerExtensions
{
    public static T ConfigurationAs<T>(this IPropertyType propertyType, IDataTypeService dataTypeService)
        where T : class
    {
        var dataType = dataTypeService.GetDataType(propertyType.DataTypeId)
            ?? throw new InvalidOperationException("Could not find the datatype that is associated with the given property type");
        
        var config = dataType.ConfigurationAs<T>()
            ?? throw new InvalidOperationException("Could not read the configuration of the given property type because it is of an unexpected type");
        
        return config;
    }
}