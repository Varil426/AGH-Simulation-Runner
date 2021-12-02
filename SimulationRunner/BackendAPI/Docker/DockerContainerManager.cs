using Application.Interfaces;
using Docker.DotNet;
using Docker.DotNet.Models;
using Domain;
using Persistence;
using SimulationHandler;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text.Json;

namespace BackendAPI.Docker;

public class DockerContainerManager : IDockerContainerManager
{
    private readonly Dictionary<User, List<ContainerNode>> _containers = new();

    private readonly DockerClient _dockerClient;
    private readonly IConfiguration _configuration;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ISimulationHandler _simulationHandler;
    private readonly string containersDirectoriesRootPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)!, "containers");

    public DockerContainerManager(IConfiguration configuration, IServiceScopeFactory serviceScopeFactory, ISimulationHandler simulationHandler)
    {
        // TODO Get URI form configuration
        _configuration = configuration;
        _serviceScopeFactory = serviceScopeFactory;
        _simulationHandler = simulationHandler;
        _dockerClient = new DockerClientConfiguration().CreateClient();
    }

    public IReadOnlyDictionary<User, List<ContainerNode>> UsersContainers => new ReadOnlyDictionary<User, List<ContainerNode>>(_containers);

    public List<ContainerNode> FindUserContainers(User user) => _containers.TryGetValue(user, out var containers) ? containers : new List<ContainerNode>();

    public async Task RunSimulationAsync(Simulation simulation, Dictionary<string, JsonElement> parameters)
    {
        using var dataContext = _serviceScopeFactory.CreateAsyncScope().ServiceProvider.GetService(typeof(DataContext)) as DataContext ?? throw new Exception();

        var simulationRunAttempt = new SimulationRunAttempt { Simulation = simulation, AttemptNumer = simulation.SimulationRunAttempts.Count + 1, Id = Guid.NewGuid(), Start = DateTime.Now };
        simulation.SimulationRunAttempts.Add(simulationRunAttempt);

        var containerId = simulationRunAttempt.Id;
        var containerDataPath = CreateContainerFileStructure(containerId.ToString());
        PrepareSimulationFiles(containerDataPath, simulation, parameters);
        // TODO Implement runner
        // TODO Implement background task watching over conteiners -> store results

        await dataContext.SaveChangesAsync();
    }

    /// <summary>
    /// Creates directory structure for new container.
    /// </summary>
    /// <param name="containerId">Conatiner ID.</param>
    /// <returns>Path to container directory.</returns>
    private string CreateContainerFileStructure(string containerId)
    {
        if (!Directory.Exists(containersDirectoriesRootPath))
            Directory.CreateDirectory(containersDirectoriesRootPath);

        var containerDirectoryPath = Path.Combine(containersDirectoriesRootPath, containerId);
        Directory.CreateDirectory(containerDirectoryPath);

        return containerDirectoryPath;
    }

    private async void PrepareSimulationFiles(string directoryPath, Simulation simulation, Dictionary<string, JsonElement> parameters)
    {
        // Place simulation file
        var simulationFilePath = Path.ChangeExtension(Path.Combine(directoryPath, ISimulationHandler.SimulationFileFileName), simulation.FileType.ToString().ToLower());
        using var simulationFileFileStream = new FileStream(simulationFilePath, FileMode.OpenOrCreate);
        var simulationFileTask = simulationFileFileStream.WriteAsync(simulation.Files);

        // Place parameters template
        var simulationParametersTemplatePath = Path.ChangeExtension(Path.Combine(directoryPath, ISimulationHandler.SimulationParametersTemplateFileName), ISimulationHandler.JsonFileExtension);
        using var simulationParamsTemplateFileStream = new FileStream(simulationParametersTemplatePath, FileMode.OpenOrCreate);
        using var simulationParamsTemplateStreamWriter = new StreamWriter(simulationParamsTemplateFileStream);
        var parametersTemplateTask = simulationParamsTemplateStreamWriter.WriteAsync(JsonSerializer.Serialize(simulation.SimulationParamsTemplate.ToDictionary(x => x.Name, x => x.Type)));

        // Place results template
        var simulationResultsTemplatePath = Path.ChangeExtension(Path.Combine(directoryPath, ISimulationHandler.SimulationResultsTemplateFileName), ISimulationHandler.JsonFileExtension);
        using var simulationResultsTemplateFileStream = new FileStream(simulationResultsTemplatePath, FileMode.OpenOrCreate);
        using var simulationResultsTemplateStreamWriter = new StreamWriter(simulationResultsTemplateFileStream);
        var resultsTemplateTask = simulationResultsTemplateStreamWriter.WriteAsync(JsonSerializer.Serialize(simulation.SimulationResultsTemplate.ToDictionary(x => x.Name, x => x.Type)));

        // Place parameters
        var simulationParametersPath = Path.ChangeExtension(Path.Combine(directoryPath, ISimulationHandler.SimulationParametersFileName), ISimulationHandler.JsonFileExtension);
        using var simulationParametersFileStream = new FileStream(simulationParametersPath, FileMode.OpenOrCreate);
        using var simulationParamsStreamWriter = new StreamWriter(simulationParametersFileStream);
        var parametersTask = simulationParamsStreamWriter.WriteAsync(JsonSerializer.Serialize(parameters));

        // Await tasks
        await simulationFileTask;
        await parametersTemplateTask;
        await resultsTemplateTask;
        await parametersTask;

        // Close streams - automatically closed in Dispose
    }
}