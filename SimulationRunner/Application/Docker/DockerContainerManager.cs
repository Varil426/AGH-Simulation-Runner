using Application.Interfaces;
using Docker.DotNet;
using Docker.DotNet.Models;
using Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimulationHandler;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text.Json;

namespace Application.Docker;

public class DockerContainerManager : IDockerContainerManager
{
    private const string CONTAINTER_DATA_DIRECTORY_PATH = "/data";

    private readonly Dictionary<Domain.User, List<(string ContainerId, SimulationRunAttempt RunAttempt)>> _containers = new();

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

        _dockerImageId = configuration.GetValue<string>("SimulationRunnerServiceDockerImage");

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

    public IReadOnlyDictionary<Domain.User, List<(string ContainerId, SimulationRunAttempt RunAttempt)>> UsersContainers => new ReadOnlyDictionary<Domain.User, List<(string, SimulationRunAttempt)>>(_containers);

    public HashSet<string> FindUserContainers(Domain.User user) => new(_containers.TryGetValue(user, out var containers) ? containers.Select(x => x.ContainerId) : Enumerable.Empty<string>());

    public async Task RunSimulationAsync(Domain.Simulation simulation, DataContext dataContext,  Dictionary<string, JsonElement> parameters)
    {
        var simulationRunAttempt = new SimulationRunAttempt { Simulation = simulation, AttemptNumer = simulation.SimulationRunAttempts.Count + 1, Id = Guid.NewGuid(), Start = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc) }; // dataContext.CreateProxy<SimulationRunAttempt>() 
        /*var simulationRunAttempt = dataContext.CreateProxy<SimulationRunAttempt>();
        simulationRunAttempt.Simulation = simulation;
        simulationRunAttempt.AttemptNumer = simulation.SimulationRunAttempts.Count + 1;
        simulationRunAttempt.Id = Guid.NewGuid();
        simulationRunAttempt.Start = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);*/

        dataContext.RunAttempts.Add(simulationRunAttempt); // TODO Check out why it has to be done that way. It should work without this. (Adding Related Entities; The database operation was expected to affect 1 row(s), but actually affected 0 row(s); Multithreaded EF)
        simulation.SimulationRunAttempts.Add(simulationRunAttempt);

        var containerName = simulationRunAttempt.Id.ToString();
        var containerDataPath = CreateContainerFileStructure(containerName);
        await PrepareSimulationFiles(containerDataPath, simulation, parameters);

        var containerId = await CreateNewContainer(GetContainerDataPath(containerName), containerName);
        AddUserContainer(simulation.User, containerId, simulationRunAttempt);
        await RunContainer(containerId);
        await dataContext.SaveChangesAsync();
    }

    public async Task RunSimulationAsync(Guid simulationId, Dictionary<string, JsonElement> parameters)
    {
        using var dataContext =  CreateDataContext();
        var simulation = await dataContext.Simulations.FindAsync(simulationId) ?? throw new Exception("Simulation not found");

        await RunSimulationAsync(simulation, dataContext, parameters);
    }


    /// <summary>
    /// Creates new container.
    /// </summary>
    /// <param name="containerDataPath">Path to container data.</param>
    /// <returns>Returns container ID.</returns>
    private async Task<string> CreateNewContainer(string containerDataPath, string name)
    {
        var parameters = new CreateContainerParameters
        {
            Image = _dockerImageId,
            Name = name,
            NetworkDisabled = true,
            HostConfig = new() { Mounts = new List<Mount>() },
            //Entrypoint = new List<string> { "dotnet", "SimulationRunnerService.dll" }
            StopTimeout = TimeSpan.FromMinutes(30), // TODO Change
            Env = new List<string> { $"SIMULATION_RUNNER_SERVICE_DATA_PATH={CONTAINTER_DATA_DIRECTORY_PATH}" }
        };

        parameters.HostConfig.Mounts.Add(new Mount { Source = containerDataPath, Target = CONTAINTER_DATA_DIRECTORY_PATH, Type = "bind"});

        var result = await _dockerClient.Containers.CreateContainerAsync(parameters);
        return result.ID;
    }

    private async Task RunContainer(string containerId) => await _dockerClient.Containers.StartContainerAsync(containerId, new ContainerStartParameters());

    private DataContext CreateDataContext() => _serviceScopeFactory.CreateAsyncScope().ServiceProvider.GetService(typeof(DataContext)) as DataContext ?? throw new Exception();

    /// <summary>
    /// Creates directory structure for new container.
    /// </summary>
    /// <param name="containerName">Conatiner ID.</param>
    /// <returns>Path to container directory.</returns>
    private string CreateContainerFileStructure(string containerName)
    {
        if (!Directory.Exists(_containersDirectoriesRootPath))
            Directory.CreateDirectory(_containersDirectoriesRootPath);

        var containerDirectoryPath = GetContainerDataPath(containerName);
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

    private void AddUserContainer(Domain.User user, string containerId, SimulationRunAttempt simulationRunAttempt)
    {
        if (_containers.ContainsKey(user) && _containers[user] != null)
            _containers[user].Add((containerId, simulationRunAttempt));
        else
            _containers[user] = new() { (containerId, simulationRunAttempt) };
    }
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
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

    public Domain.User? GetContainerOwner(string containerId) => _containers.FirstOrDefault(x => x.Value.Select(x => x.ContainerId).Contains(containerId)).Key;

    public async Task<IReadOnlyCollection<ContainerListResponse>> GetAllUserContainersStats(Domain.User user) => new ReadOnlyCollection<ContainerListResponse>
        ((await _dockerClient.Containers.ListContainersAsync(new ContainersListParameters { All = true }))
        .Where(x => _containers[user].Any(y => y.ContainerId == x.ID)).ToList());

    public async Task<IReadOnlyCollection<ContainerListResponse>> GetAllUserContainersStats() => new ReadOnlyCollection<ContainerListResponse>
        ((await _dockerClient.Containers.ListContainersAsync(new ContainersListParameters { All = true }))
        .Where(x => _containers.Values.SelectMany(y => y.Select(z => z.ContainerId)).Contains(x.ID)).ToList());

    public async Task<IReadOnlyDictionary<SimulationRunAttempt, ContainerListResponse>> GetAllUserContainersStatsPerRunAttempt()
        => _containers.Values.SelectMany(x => x)
        .Join(
            await GetAllUserContainersStats(),
            x => x.ContainerId, 
            x => x.ID, 
            (containerMetaData, containerStats) => (RunAttempt: containerMetaData.RunAttempt, ContainerStats: containerStats))
        .ToDictionary(x => x.RunAttempt, x => x.ContainerStats);

    public string GetContainerDataPath(string containerName) => Path.Combine(_containersDirectoriesRootPath, containerName);

    ~DockerContainerManager() => Dispose(false);
}