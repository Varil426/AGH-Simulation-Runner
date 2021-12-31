using System.Collections.Immutable;

namespace SimulationStandard;

/// <summary>
/// Helper class for type handling.
/// </summary>
public static class TypesHelper
{
    static TypesHelper()
    {
        AllowedTypes = ImmutableHashSet.CreateRange(new HashSet<Type>
        {
            typeof(long),
            typeof(double),
            typeof(bool),
            typeof(string),
            typeof(IList<long>),
            typeof(IList<double>),
            typeof(IList<bool>),
            typeof(IList<string>),
        });
    }

    /// <summary>
    /// All allowed types.
    /// </summary>
    public static IReadOnlySet<Type> AllowedTypes { get; }

    /// <summary>
    /// Checks whether this type of parameter is allowed.
    /// </summary>
    /// <param name="value">Obcjet to be checked.</param>
    /// <returns><see langword="true"/> if type is allowed; <see langword="false"/> otherwise.</returns>
    public static bool CheckIfAllowedType(Type type) => AllowedTypes.Any(allowedType => type.IsAssignableTo(allowedType));
}
