using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain;

public class Simulation
{
    public Simulation(Guid id, string name, byte[] files)
    {
        Id = id;
        Name = name;
        Files = files;
        SimulationResults = new Collection<SimulationRunAttempt>();
        SimulationResultsTemplate = new Collection<SimulationResultTemplate>();
        GetSimulationParamsTemplate = new Collection<SimulationParamTemplate>();
    }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

    public Guid Id { get; set; }

    [Required]
    public virtual User User { get; set; } = null!;

    [Required]
    public string Name { get; set; }

    public string? Version { get; set; }

    public byte[] Files { get; set; }

    public virtual ICollection<SimulationRunAttempt> SimulationResults { get; set; }

    public virtual ICollection<SimulationResultTemplate> SimulationResultsTemplate { get; set; }

    public virtual ICollection<SimulationParamTemplate> GetSimulationParamsTemplate { get; set; }
}