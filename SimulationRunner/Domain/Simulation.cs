namespace Domain;

public class Simulation
{
    public enum AllowedFileTypesEnum
    {
        Uknown,
        DLL,
        ZIP
    }

    public Simulation(string name, byte[] files, AllowedFileTypesEnum fileType)
    {
        Name = name;
        Files = files;
        SimulationResults = new Collection<SimulationRunAttempt>();
        SimulationResultsTemplate = new Collection<SimulationResultTemplate>();
        GetSimulationParamsTemplate = new Collection<SimulationParamTemplate>();
        FileType = fileType;
    }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

    public Guid Id { get; set; }

    [Required]
    public virtual User User { get; set; } = null!;

    [Required]
    public string Name { get; set; }

    public string? Version { get; set; }

    [Required]
    public byte[] Files { get; set; }

    [Required]
    public AllowedFileTypesEnum FileType { get; set; }

    public virtual ICollection<SimulationRunAttempt> SimulationResults { get; set; }

    public virtual ICollection<SimulationResultTemplate> SimulationResultsTemplate { get; set; }

    public virtual ICollection<SimulationParamTemplate> GetSimulationParamsTemplate { get; set; }
}