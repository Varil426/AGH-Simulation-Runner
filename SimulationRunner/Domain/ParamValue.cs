namespace Domain;

public class ParamValue : ValueBase
{
    [Required]
    public virtual SimulationParamTemplate SimulationParamTemplate { get; set; } = null!;
}
