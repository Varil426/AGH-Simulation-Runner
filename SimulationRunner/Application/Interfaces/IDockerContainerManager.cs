using Docker.DotNet.Models;
using System.Text.Json;

namespace Application.Interfaces;

public interface IDockerContainerManager
{
    public IReadOnlyDictionary<Domain.User, List<ContainerNode>> UsersContainers { get; }

    public List<ContainerNode> FindUserContainers(Domain.User user);

    public void RunSimulation(Domain.Simulation simulation, Dictionary<string, JsonElement> parameters);

    // TODO Rest
}