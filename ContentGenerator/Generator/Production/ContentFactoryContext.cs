/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
using Umbraco.Cms.Core.Models;

namespace ContentGenerator.Generator.Production;

public record ContentFactoryContext(
    IContent Parent,
    IContentType TargetContentType,
    int UserId,
    IGeneratorContext GeneratorContext
);