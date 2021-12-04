using Docker.DotNet.Models;
using System.Text.Json;

namespace Application.Interfaces;

public interface IDockerContainerManager : IDisposable
{
    public IReadOnlyDictionary<Domain.User, List<ContainerNode>> UsersContainers { get; }

    public List<ContainerNode> FindUserContainers(Domain.User user);

    public Task RunSimulationAsync(Domain.Simulation simulation, Dictionary<string, JsonElement> parameters);

    // TODO Rest
}