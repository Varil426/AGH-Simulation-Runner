using SimulationStandard;
using System.Collections;

namespace Domain;

public abstract class ValuesTemplate
{
    public ValuesTemplate(string name, string type)
    {
        Name = name;
        Type = type;
    }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [Required]
    public virtual Simulation Simulation { get; set; } = null!;

    public string Name { get; set; }

    public string Type { get; set; }

    public Type TypeAsType => TypesHelper.AllowedTypes.FirstOrDefault(x => x.ToString() == Type) ?? throw new Exception("Disallowed Type");

    public bool IsCollection => TypeAsType.IsGenericType && TypeAsType.GetGenericTypeDefinition() == typeof(IList<>);
}
