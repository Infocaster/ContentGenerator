/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
namespace ContentGenerator.Generator;

public interface IGeneratorContext
{
    object? this[string key] { get; set; }
    
    T Get<T>(string key)
        where T : notnull;

    void Set(string key, object? value);
}

public class GeneratorContext
    : IGeneratorContext
{
    private readonly Dictionary<string, object> content = [];

    public object? this[string key]
    {
        get => content.TryGetValue(key, out var value) ? value : null;
        set { if (value is not null) content[key] = value; else content.Remove(key); }
    }

    public bool Has(string key) => content.ContainsKey(key);

    public T Get<T>(string key) where T : notnull
    {
        return (T)(this[key] ?? throw new ArgumentOutOfRangeException(nameof(key)));
    }

    public void Set(string key, object? value)
    {
        this[key] = value;
    }

    public void Merge(IDictionary<string, object> valueSet)
    {
        foreach(var kvp in valueSet)
        {
            content[kvp.Key] = kvp.Value;
        }
    }
}