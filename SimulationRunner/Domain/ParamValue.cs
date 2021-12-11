namespace Domain;

public class ParamValue : ValueBase<ValueOfParamCollection>
{
    [Required]
    public virtual SimulationParamTemplate SimulationParamTemplate { get; set; } = null!;

    public override bool IsCollection => SimulationParamTemplate.IsCollection;
}
