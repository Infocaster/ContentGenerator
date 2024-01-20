/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
using Umbraco.Cms.Core.Models;

namespace ContentGenerator.Generator.Enrichment;

public interface IPropertyFiller
{
    IPropertySink FillProperties(IPropertySink content, IGeneratorContext context);
}

public interface IReusablePropertyFiller
    : IPropertyFiller
{
    IPropertyFiller Reuse(IPropertyType propertyType);
}

public interface IPropertyFillerFactory
{
    ValueTask<IReadOnlyCollection<IPropertyFiller>> CreateAsync(PropertyFillerContext context);
}