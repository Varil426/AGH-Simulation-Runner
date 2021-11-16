using System.ComponentModel.DataAnnotations.Schema;

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

    // TODO Add computed property Type TypeAsType, bool IsCollection
}
