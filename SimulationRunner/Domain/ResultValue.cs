namespace Domain;

public class ResultValue : ValueBase<ValueOfResultCollection>
{
    [Required]
    public virtual SimulationResultTemplate SimulationResultTemplate { get; set; } = null!;

    public override bool IsCollection => SimulationResultTemplate.IsCollection;
}
