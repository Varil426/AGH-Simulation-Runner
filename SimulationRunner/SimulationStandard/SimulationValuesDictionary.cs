using SimulationStandard.Exceptions;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace SimulationStandard;

/// <summary>
/// This class defines parameters used by simulation.
/// </summary>
public sealed class SimulationValuesDictionary : IDictionary<string, object>
{
    private readonly Dictionary<string, object> _params = new();

    /// <summary>
    /// Returns value for a given key.
    /// </summary>
    /// <param name="key">Key.</param>
    /// <returns>Value for a given key.</returns>
    /// <exception cref="UnsupportedTypeException">Unsupported type.</exception>
    public object this[string name]
    {
        get => _params[name];

        set
        {
            if (TypesHelper.CheckIfAllowedType(value.GetType()))
            {
                _params[name] = value;
            }
            else
            {
                throw new UnsupportedTypeException { ReceivedType = value.GetType()};
            }
        }
    }

    /// <summary>
    /// Returns value for a given key.
    /// </summary>
    /// <param name="key">Key.</param>
    /// <returns>Value for a given key.</returns>
    public object this[int index]
    {
        get => _params.ElementAt(index);
    }

    /// <summary>
    /// Gets all types stored in this <see cref="SimulationValuesDictionary"/>.
    /// </summary>
    public IEnumerable<(string Name, Type Type)> ContainedTypes => _params.Select(x => (x.Key, x.Value.GetType()));

    public ICollection<string> Keys => _params.Keys;

    public ICollection<object> Values => _params.Values;

    public int Count => _params.Count;

    public bool IsReadOnly => false;

    public void Add(string key, object value) => this[key] = value;

    public void Add(KeyValuePair<string, object> item) => Add(item.Key, item.Value);

    public void Clear() => _params.Clear();

    public bool Contains(KeyValuePair<string, object> item) => _params.Contains(item);

    public bool ContainsKey(string key) => _params.ContainsKey(key);

    void ICollection<KeyValuePair<string, object>>.CopyTo(KeyValuePair<string, object>[] array, int arrayIndex) => ((ICollection<KeyValuePair<string, object>>)_params).CopyTo(array, arrayIndex);

    public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => _params.GetEnumerator();

    public bool Remove(string key) => _params.Remove(key);

    bool ICollection<KeyValuePair<string, object>>.Remove(KeyValuePair<string, object> item) => ((ICollection<KeyValuePair<string, object>>)_params).Remove(item);

    public bool TryGetValue(string key, [MaybeNullWhen(false)] out object value) => _params.TryGetValue(key, out value);

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<KeyValuePair<string, object>>)_params).GetEnumerator();
}
