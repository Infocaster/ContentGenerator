/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
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