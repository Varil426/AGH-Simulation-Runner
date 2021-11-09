namespace SimulationStandard.Exceptions;

[Serializable]
public class UnsupportedTypeException : Exception
{
    public UnsupportedTypeException() { }

    public UnsupportedTypeException(string message) : base(message) { }

    /// <summary>
    /// Received type.
    /// </summary>
    public Type? ReceivedType { get; init; }
}