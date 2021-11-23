namespace Application.Simulation;

public class SimulationDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Version { get; set; }
}