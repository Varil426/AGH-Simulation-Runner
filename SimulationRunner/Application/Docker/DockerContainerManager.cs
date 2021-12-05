using Application.Interfaces;
using Docker.DotNet;
using Docker.DotNet.Models;
using Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimulationHandler;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;

namespace Application.Docker;

public class DockerContainerManager : IDockerContainerManager
{
    private readonly Dictionary<Domain.User, List<ContainerNode>> _containers = new();

    private bool _disposed;

    private readonly DockerClient _dockerClient;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ISimulationHandler _simulationHandler;
    //private readonly IConfiguration _configuration;
    private readonly string _containersDirectoriesRootPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)!, "containers");
    //private readonly string _simulationRunnerServicePath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)!, "SimulationRunnerService");
    //private readonly string _simulationRunnerServiceDockerfileName = "DockerfileDockerApi";
    //private readonly string _dockerRepository = "simulationrunnerservice";
    private readonly string _dockerImageId;

    public DockerContainerManager(IServiceScopeFactory serviceScopeFactory, ISimulationHandler simulationHandler, IConfiguration configuration)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _simulationHandler = simulationHandler;
        //_configuration = configuration;
        // TODO Get URI form configuration if need be
        _dockerClient = new DockerClientConfiguration().CreateClient();

        _dockerClient.DefaultTimeout = TimeSpan.FromSeconds(30);

        _dockerImageId = configuration.GetValue<string>("SimulationRunnerServiceDockerImageId");

        // TODO Dynamic Docker Image Creation
        /*
        var tarArchivePathTask = FileHelper.CreateTarArchive(_simulationRunnerServicePath);
        tarArchivePathTask.Wait();
        var tarArchivePath = tarArchivePathTask.Result;

        using var fileStream = new FileStream(tarArchivePath, FileMode.Open);

        var buildParameters = new ImageBuildParameters
        {
            Dockerfile = _simulationRunnerServiceDockerfileName,
        };
        buildParameters.Tags = new List<string> { $"{_dockerRepository}:latest" };
        var imageCreation = _dockerClient.Images.BuildImageFromDockerfileAsync(buildParameters, fileStream, null, null, new Progress<JSONMessage>(json =>
        {
            if (json.ErrorMessage != null)
                throw new Exception();
        }));

        imageCreation.Wait();
        */
    }

    public IReadOnlyDictionary<Domain.User, List<ContainerNode>> UsersContainers => new ReadOnlyDictionary<Domain.User, List<ContainerNode>>(_containers);

    public List<ContainerNode> FindUserContainers(Domain.User user) => _containers.TryGetValue(user, out var containers) ? containers : new List<ContainerNode>();

    public async Task RunSimulationAsync(Domain.Simulation simulation, Dictionary<string, JsonElement> parameters)
    {
        using var dataContext = _serviceScopeFactory.CreateAsyncScope().ServiceProvider.GetService(typeof(DataContext)) as DataContext ?? throw new Exception();

        var simulationRunAttempt = new SimulationRunAttempt { Simulation = simulation, AttemptNumer = simulation.SimulationRunAttempts.Count + 1, Id = Guid.NewGuid(), Start = DateTime.Now };
        simulation.SimulationRunAttempts.Add(simulationRunAttempt);

        var containerDataPath = CreateContainerFileStructure(simulationRunAttempt.Id.ToString());
        await PrepareSimulationFiles(containerDataPath, simulation, parameters);

        var containerId = await CreateNewContainer(containerDataPath, simulationRunAttempt.Id.ToString());
        await RunContainer(containerId);

        // TODO Implement runner
        // TODO Implement background task watching over conteiners -> store results

        await dataContext.SaveChangesAsync();
    }

    /// <summary>
    /// Creates new container.
    /// </summary>
    /// <param name="containerDataPath">Path to container data.</param>
    /// <returns>Returns container ID.</returns>
    private async Task<string> CreateNewContainer(string containerDataPath, string? name = null)
    {
        var parameters = new CreateContainerParameters
        {
            Image = _dockerImageId,
            Name = name ?? string.Empty,
            NetworkDisabled = true,
            HostConfig = new() { Mounts = new List<Mount>() },
            //Entrypoint = new List<string> { "dotnet", "SimulationRunnerService.dll" }
            StopTimeout = TimeSpan.FromMinutes(30), // TODO Change
        };

        parameters.HostConfig.Mounts.Add(new Mount { Source = containerDataPath, Target = "/data", Type = "bind"});

        var result = await _dockerClient.Containers.CreateContainerAsync(parameters);
        return result.ID;
    }

    private async Task RunContainer(string containerId) => await _dockerClient.Containers.StartContainerAsync(containerId, new ContainerStartParameters());

    /// <summary>
    /// Creates directory structure for new container.
    /// </summary>
    /// <param name="containerId">Conatiner ID.</param>
    /// <returns>Path to container directory.</returns>
    private string CreateContainerFileStructure(string containerId)
    {
        if (!Directory.Exists(_containersDirectoriesRootPath))
            Directory.CreateDirectory(_containersDirectoriesRootPath);

        var containerDirectoryPath = Path.Combine(_containersDirectoriesRootPath, containerId);
        Directory.CreateDirectory(containerDirectoryPath);

        return containerDirectoryPath;
    }

    private async Task PrepareSimulationFiles(string directoryPath, Domain.Simulation simulation, Dictionary<string, JsonElement> parameters)
    {
        // TODO Add factories

        // Place simulation file
        var simulationFilePath = Path.ChangeExtension(Path.Combine(directoryPath, ISimulationHandler.SimulationFileFileName), simulation.FileType.ToString().ToLower());
        using var simulationFileFileStream = new FileStream(simulationFilePath, FileMode.OpenOrCreate);
        var simulationFileTask = simulationFileFileStream.WriteAsync(simulation.Files);

        // Place parameters template
        var simulationParametersTemplatePath = Path.ChangeExtension(Path.Combine(directoryPath, ISimulationHandler.SimulationParametersTemplateFileName), ISimulationHandler.JsonFileExtension);
        using var simulationParamsTemplateFileStream = new FileStream(simulationParametersTemplatePath, FileMode.OpenOrCreate);
        using var simulationParamsTemplateStreamWriter = new StreamWriter(simulationParamsTemplateFileStream);
        var simulationParamsTemplate = new SimulationStandard.SimulationParamsTemplate();
        foreach (var param in simulation.SimulationParamsTemplate)
            simulationParamsTemplate[param.Name] = param.TypeAsType;
        var parametersTemplateTask = simulationParamsTemplateStreamWriter.WriteAsync(_simulationHandler.ToJson(simulationParamsTemplate));

        // Place results template
        var simulationResultsTemplatePath = Path.ChangeExtension(Path.Combine(directoryPath, ISimulationHandler.SimulationResultsTemplateFileName), ISimulationHandler.JsonFileExtension);
        using var simulationResultsTemplateFileStream = new FileStream(simulationResultsTemplatePath, FileMode.OpenOrCreate);
        using var simulationResultsTemplateStreamWriter = new StreamWriter(simulationResultsTemplateFileStream);
        var simulationResultsTemplate = new SimulationStandard.SimulationResultsTemplate();
        foreach (var param in simulation.SimulationResultsTemplate)
            simulationResultsTemplate[param.Name] = param.TypeAsType;
        var resultsTemplateTask = simulationResultsTemplateStreamWriter.WriteAsync(_simulationHandler.ToJson(simulationResultsTemplate));

        // Place parameters
        var simulationParametersPath = Path.ChangeExtension(Path.Combine(directoryPath, ISimulationHandler.SimulationParametersFileName), ISimulationHandler.JsonFileExtension);
        using var simulationParametersFileStream = new FileStream(simulationParametersPath, FileMode.OpenOrCreate);
        using var simulationParamsStreamWriter = new StreamWriter(simulationParametersFileStream);
        var simulationParams = new SimulationStandard.SimulationParams();
        foreach (var (name, element) in parameters)
            simulationParams.Params[name] = JsonSerializer.Deserialize(element, simulationParamsTemplate[name]) ?? throw new Exception("JSON Deserialization failed");
        var parametersTask = simulationParamsStreamWriter.WriteAsync(_simulationHandler.ToJson(simulationParams));

        // Await tasks
        await simulationFileTask;
        await parametersTemplateTask;
        await resultsTemplateTask;
        await parametersTask;

        // Close streams - automatically closed in Dispose
    }

    public void Dispose()
    {
        if (!_disposed)
            Dispose(true);
    }

    private void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _dockerClient.Dispose();
            }

            _disposed = true;
        }
    }
    ~DockerContainerManager() => Dispose(false);
}