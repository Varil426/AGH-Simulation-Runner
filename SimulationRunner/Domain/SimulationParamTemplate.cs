namespace Domain;

public class SimulationParamTemplate : ValuesTemplate
{
    public SimulationParamTemplate(string name, string type) : base(name, type)
    {
    }

    public virtual ICollection<ParamValue> ParamValues { get; set; } = null!;
}