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

public class ContentPropertySink : IPropertySink
{
    private readonly IContent content;

    public ContentPropertySink(IContent content)
    {
        this.content = content;
    }

    public void SetValue(string alias, object? value, string? culture, string? segment)
    {
        content.SetValue(alias, value, culture, segment);
    }
}

public class DictionaryPropertySink : IPropertySink
{
    private readonly IDictionary<string, object?> dictionary;

    public DictionaryPropertySink(IDictionary<string, object?> dictionary)
    {
        this.dictionary = dictionary;
    }

    public void SetValue(string alias, object? value, string? culture, string? segment)
    {
        dictionary[alias] = value;
    }
}