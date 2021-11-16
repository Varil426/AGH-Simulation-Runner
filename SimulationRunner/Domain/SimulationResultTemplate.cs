namespace Domain;

public class SimulationResultTemplate : ValuesTemplate
{
    public SimulationResultTemplate(string name, string type) : base(name, type)
    {
    }

    public virtual ICollection<ResultValue> ResultValues { get; set; } = null!;
}