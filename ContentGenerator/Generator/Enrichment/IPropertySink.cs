/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
using Umbraco.Cms.Core.Models;

namespace ContentGenerator.Generator.Enrichment;

public interface IPropertySink
{
    void SetValue(string alias, object? value, string? culture, string? segment);
}

public class ContentPropertySink(IContent content)
        : IPropertySink
{
    public void SetValue(string alias, object? value, string? culture, string? segment)
    {
        content.SetValue(alias, value, culture, segment);
    }
}

public class DictionaryPropertySink(IDictionary<string, object?> dictionary)
        : IPropertySink
{
    public void SetValue(string alias, object? value, string? culture, string? segment)
    {
        dictionary[alias] = value;
    }
}