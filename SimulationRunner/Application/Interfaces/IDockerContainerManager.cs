using Docker.DotNet.Models;
using Domain;
using System.Text.Json;

namespace Application.Interfaces;

public interface IDockerContainerManager : IDisposable
{
    /// <summary>
    /// Lists IDs of all containers belonging to a user.
    /// </summary>
    public IReadOnlyDictionary<Domain.User, List<(string ContainerId, SimulationRunAttempt RunAttempt)>> UsersContainers { get; }

    public HashSet<string> FindUserContainers(Domain.User user);

    public Domain.User? GetContainerOwner(string containerId);

    public Task RunSimulationAsync(Domain.Simulation simulation, DataContext dataContext, Dictionary<string, JsonElement> parameters);

    public Task RunSimulationAsync(Guid simulationId, Dictionary<string, JsonElement> parameters);

    public Task<IReadOnlyCollection<ContainerListResponse>> GetAllUserContainersStats(Domain.User user);

    public Task<IReadOnlyCollection<ContainerListResponse>> GetAllUserContainersStats();

    public Task<IReadOnlyDictionary<SimulationRunAttempt, ContainerListResponse>> GetAllUserContainersStatsPerRunAttempt();

    // TODO Rest
}