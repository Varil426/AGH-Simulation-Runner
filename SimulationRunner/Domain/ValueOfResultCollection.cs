namespace Domain;

public class ValueOfResultCollection : ValueOfCollection
{
    public ValueOfResultCollection(string value) : base(value)
    {
    }

    [Required]
    public virtual ResultValue ResultValue { get; set; } = null!;
}
