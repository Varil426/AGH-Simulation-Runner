using Application.Interfaces;
using Docker.DotNet;
using Docker.DotNet.Models;
using Domain;
using Persistence;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace BackendAPI.Docker;

public class DockerContainerManager : IDockerContainerManager
{
    private readonly Dictionary<User, List<ContainerNode>> _containers = new();

    private readonly DockerClient _dockerClient;
    private readonly IConfiguration _configuration;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public DockerContainerManager(IConfiguration configuration, IServiceScopeFactory serviceScopeFactory)
    {
        // TODO Get URI form configuration
        _configuration = configuration;
        _serviceScopeFactory = serviceScopeFactory;
        _dockerClient = new DockerClientConfiguration().CreateClient();
    }

    public IReadOnlyDictionary<User, List<ContainerNode>> UsersContainers => new ReadOnlyDictionary<User, List<ContainerNode>>(_containers);

    public List<ContainerNode> FindUserContainers(User user) => _containers.TryGetValue(user, out var containers) ? containers : new List<ContainerNode>();

    public void RunSimulation(Simulation simulation, Dictionary<string, JsonElement> parameters)
    {
        // TODO
        throw new NotImplementedException();
    }
}