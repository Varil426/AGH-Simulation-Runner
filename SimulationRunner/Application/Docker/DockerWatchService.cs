using Application.Interfaces;
using Docker.DotNet.Models;
using Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SimulationHandler;
using System.Collections;

namespace Application.Docker;

public class DockerWatchService : BackgroundService, IDockerWatchService
{
    private readonly IDockerContainerManager _dockerContainerManager;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ISimulationHandler _simulationHandler;

    public DockerWatchService(IDockerContainerManager dockerContainerManager, IServiceScopeFactory serviceScopeFactory, ISimulationHandler simulationHandler)
    {
        _dockerContainerManager = dockerContainerManager;
        _serviceScopeFactory = serviceScopeFactory;
        _simulationHandler = simulationHandler;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await PerformTask();
            //Thread.Sleep(TimeSpan.FromSeconds(30)); // TODO
            Thread.Sleep(TimeSpan.FromSeconds(5));
        }
    }

    private async Task PerformTask()
    {
        var test = (await _dockerContainerManager.GetAllUserContainersStats()).Select(x => (x.Status, x.State));
        var results = await CollectRunAttemptsResults();
        StoreSimulationResults(results);

        // TODO Remove finished containers (DockerContainerManager) and clean disk space (SimulationHandler or DockerContainerManager)

        // TODO Store run parameters
        // TODO Get container error and store it in DB
        // TODO Policy - owner of a simulation
        // TODO ZIP Handling
        // TODO Endpoints
    }

    private void StoreSimulationResults(IReadOnlyDictionary<SimulationRunAttempt, ContainerListResponse> results)
    {
        var tasks = new List<Task>();
        foreach (var (runAttempt, containerResponse) in results)
        {
            tasks.Add(StoreSimulationResult(runAttempt.Id, containerResponse));
        }

        Task.WaitAll(tasks.ToArray());
    }

    private async Task StoreSimulationResult(Guid runAttemptId, ContainerListResponse containerResponse)
    {
        // Entity Framework doesn't support parallel usage of the same DbContext, therefore we should create new context for each parallel operation.
        using var dataContext = _serviceScopeFactory.CreateAsyncScope().ServiceProvider.GetService(typeof(DataContext)) as DataContext ?? throw new Exception();
        var runAttempt = await dataContext.RunAttempts.FirstOrDefaultAsync(x => x.Id == runAttemptId) ?? throw new Exception("Run Attempt not found.");
        var containerDataPath = _dockerContainerManager.GetContainerDataPath(containerResponse.Names.FirstOrDefault()?.Replace("/", string.Empty) ?? throw new Exception("Missing Container Name"));
        var resultsFilePath = Path.ChangeExtension(Path.Combine(containerDataPath, ISimulationHandler.SimulationResultsFileName), ISimulationHandler.JsonFileExtension);
        try
        {
            using var fileStream = new FileStream(resultsFilePath, FileMode.Open);
            using var streamReader = new StreamReader(fileStream);
            var results = _simulationHandler.CreateSimulationResults(await streamReader.ReadToEndAsync());

            // TODO Should be moved to different class
            foreach (var resultValue in results.Results)
            {
                var resultTemplate = runAttempt.Simulation.SimulationResultsTemplate.FirstOrDefault(x => x.Name == resultValue.Key) ?? throw new Exception("Result Template not found");

                var result = new ResultValue
                {
                    SimulationResultTemplate = resultTemplate
                };

                string? value;
                if (resultTemplate.IsCollection)
                {
                    value = null;
                    if (resultValue.Value is IEnumerable list)
                    {
                        foreach (var listValue in list)
                        {
                            var valueOfCollection = new ValueOfResultCollection(listValue.ToString()!)
                            {
                                ResultValue = result,
                            };
                            dataContext.ValuesOfResultCollections.Add(valueOfCollection);
                        }
                    }
                }
                else
                    value = resultValue.Value.ToString();

                result.Value = value;
                runAttempt.ResultValues.Add(result);
                dataContext.ResultValues.Add(result);
            }
        }
        catch (FileNotFoundException)
        {
            // TODO Error simulation exited without results? - Add error
        }

        await dataContext.SaveChangesAsync();
    }

    /// <summary>
    /// Collects all run attempts which finished.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    private async Task<IReadOnlyDictionary<SimulationRunAttempt, ContainerListResponse>> CollectRunAttemptsResults() => (await _dockerContainerManager.GetAllUserContainersStatsPerRunAttempt())
        .Where(x => x.Value.State == "exited")
        .ToDictionary(x => x.Key, x => x.Value);
}