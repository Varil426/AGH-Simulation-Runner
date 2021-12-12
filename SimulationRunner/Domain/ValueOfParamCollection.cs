namespace Domain;

public class ValueOfParamCollection : ValueOfCollection
{
    public ValueOfParamCollection(string value, int index) : base(value, index)
    {
    }

    [Required]
    public virtual ParamValue ParamValue { get; set; } = null!;
}
