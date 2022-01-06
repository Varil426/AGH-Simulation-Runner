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

    public Task<SimulationRunAttempt> RunSimulationAsync(Domain.Simulation simulation, DataContext dataContext, Dictionary<string, JsonElement> parameters);

    public Task<SimulationRunAttempt> RunSimulationAsync(Guid simulationId, Dictionary<string, JsonElement> parameters);

    public Task<IReadOnlyCollection<ContainerListResponse>> GetAllUserContainersStats(Domain.User user);

    public Task<IReadOnlyCollection<ContainerListResponse>> GetAllUserContainersStats();

    public Task<IReadOnlyDictionary<SimulationRunAttempt, ContainerListResponse>> GetAllUserContainersStatsPerRunAttempt();

    public string GetContainerDataPath(string containerName);

    public Task RemoveContainer(string containerName);

    public Task CleanAfterContainer(string containerName);

    public Task CleanAndRemoveContainer(string containerName);
}