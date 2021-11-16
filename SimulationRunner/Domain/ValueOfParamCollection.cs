namespace Domain;

public class ValueOfParamCollection : ValueOfCollection
{
    public ValueOfParamCollection(string value) : base(value)
    {
    }

    [Required]
    public virtual ParamValue ParamValue { get; set; } = null!;
}
