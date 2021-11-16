namespace Domain;

public class ResultValue : ValueBase
{
    [Required]
    public virtual SimulationResultTemplate SimulationResultTemplate { get; set; } = null!;
}
