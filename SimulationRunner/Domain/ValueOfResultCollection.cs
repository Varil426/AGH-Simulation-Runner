namespace Domain;

public class ValueOfResultCollection : ValueOfCollection
{
    public ValueOfResultCollection(string value, int index) : base(value, index)
    {
    }

    [Required]
    public virtual ResultValue ResultValue { get; set; } = null!;
}
