namespace SimulationStandard;

/// <summary>
/// This class defines parameters used by simulation.
/// </summary>
public sealed class SimulationValuesDictionary
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

    /// <summary>
    /// Checks whether this type of parameter is allowed.
    /// </summary>
    /// <param name="value">Obcjet to be checked.</param>
    /// <returns><see langword="true"/> if type is allowed; <see langword="false"/> otherwise.</returns>
    private static bool CheckIfAllowedType(Type type) => _allowedTypes.Any(type => type.IsAssignableTo(type));
}
