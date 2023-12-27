using Umbraco.Cms.Core.Models;

namespace RandomContentGenerator.Generator.Enrichment;

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