using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace SimulationStandard;

/// <summary>
/// This class defines parameters used by simulation.
/// </summary>
public sealed class SimulationValuesDictionary : IDictionary<string, object>
{
    private static readonly HashSet<Type> _allowedTypes = new()
    {
        typeof(short),
        typeof(int),
        typeof(long),
        typeof(float),
        typeof(double),
        typeof(decimal),
        typeof(bool),
        typeof(string),
        typeof(IList<short>),
        typeof(IList<int>),
        typeof(IList<long>),
        typeof(IList<float>),
        typeof(IList<double>),
        typeof(IList<decimal>),
        typeof(IList<bool>),
        typeof(IList<string>),
    };

    private readonly Dictionary<string, object> _params = new();

    /// <summary>
    /// Returns value for a given key.
    /// </summary>
    /// <param name="key">Key.</param>
    /// <returns>Value for a given key.</returns>
    public object this[string name]
    {
        get => _params[name];

        set
        {
            if (CheckIfAllowedType(value.GetType()))
            {
                _params[name] = value;
            }
            else
            {
                // TODO Throw Exception
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

    public ICollection<string> Keys => throw new NotImplementedException();

    public ICollection<object> Values => throw new NotImplementedException();

    public int Count => _params.Count;

    public bool IsReadOnly => false;

    /// <summary>
    /// Checks whether this type of parameter is allowed.
    /// </summary>
    /// <param name="value">Obcjet to be checked.</param>
    /// <returns><see langword="true"/> if type is allowed; <see langword="false"/> otherwise.</returns>
    private static bool CheckIfAllowedType(Type type) => _allowedTypes.Any(type => type.IsAssignableTo(type));

    public void Add(string key, object value) => this[key] = value;

    public void Add(KeyValuePair<string, object> item) => Add(item.Key, item.Value);

    public void Clear() => _params.Clear();

    public bool Contains(KeyValuePair<string, object> item) => _params.Contains(item);

    public bool ContainsKey(string key) => _params.ContainsKey(key);

    public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
    {
        foreach (var kvp in _params.ToList())
        {
            array[arrayIndex++] = kvp;
        }
    }

    public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => _params.GetEnumerator();

    public bool Remove(string key) => _params.Remove(key);

    public bool Remove(KeyValuePair<string, object> item) => _params.Remove(item.Key);

    public bool TryGetValue(string key, [MaybeNullWhen(false)] out object value) => _params.TryGetValue(key, out value);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
