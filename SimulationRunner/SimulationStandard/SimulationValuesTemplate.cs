using SimulationStandard.Exceptions;
using SimulationStandard.Interfaces;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace SimulationStandard;

/// <summary>
/// Dictionary - a result parameter name to a result parameter type.
/// </summary>
public abstract class SimulationValuesTemplate : ISimulationValuesTemplate
{
    private readonly Dictionary<string, Type> _parameterNameToType = new(); 

    public Type this[string name]
    { 
        get => _parameterNameToType[name];

        set
        {
            if (TypesHelper.CheckIfAllowedType(value.GetType()))
            {
                _parameterNameToType[name] = value;
            }
            else
            {
                throw new UnsupportedTypeException { ReceivedType = value.GetType() };
            }
        }
    }

    public ICollection<string> Keys => _parameterNameToType.Keys;

    public ICollection<Type> Values => _parameterNameToType.Values;

    public int Count => _parameterNameToType.Count;

    public bool IsReadOnly => false;

    public void Add(string key, Type value) => this[key] = value;

    public void Add(KeyValuePair<string, Type> item) => Add(item.Key, item.Value);

    public void Clear() => _parameterNameToType.Clear();

    public bool Contains(KeyValuePair<string, Type> item) => _parameterNameToType.Contains(item);

    public bool ContainsKey(string key) => _parameterNameToType.ContainsKey(key);

    void ICollection<KeyValuePair<string, Type>>.CopyTo(KeyValuePair<string, Type>[] array, int arrayIndex) => ((ICollection<KeyValuePair<string, Type>>)_parameterNameToType).CopyTo(array, arrayIndex);

    public IEnumerator<KeyValuePair<string, Type>> GetEnumerator() => _parameterNameToType.GetEnumerator();

    public bool Remove(string key) => _parameterNameToType.Remove(key);

    bool ICollection<KeyValuePair<string, Type>>.Remove(KeyValuePair<string, Type> item) => ((ICollection<KeyValuePair<string, Type>>)_parameterNameToType).Remove(item);

    public bool TryGetValue(string key, [MaybeNullWhen(false)] out Type value) => _parameterNameToType.TryGetValue(key, out value);

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<KeyValuePair<string, Type>>)_parameterNameToType).GetEnumerator();
}
