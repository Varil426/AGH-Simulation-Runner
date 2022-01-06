namespace SimulationRunnerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly ISimulationHandler _simulationHandler;

        public Worker(ILogger<Worker> logger, IHostApplicationLifetime hostApplicationLifetime, ISimulationHandler simulationHandler)
        {
            _logger = logger;
            _hostApplicationLifetime = hostApplicationLifetime;
            _simulationHandler = simulationHandler;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                if (!FileHelper.CheckDirectoryStructure())
                    throw new Exception("Incorrect directory structure.");

                var parametersTemplate = _simulationHandler.CreateSimulationParamsTemplate(await FileHelper.ReadFileAsync(FileHelper.SimulationParametersTemplatePath));
                var parameters = _simulationHandler.CreateSimulationParams(await FileHelper.ReadFileAsync(FileHelper.SimulationParametersPath));
                var simulationBuilder = _simulationHandler.CreateSimulationBuilder(FileHelper.SimulationFilesPath);

                var simulation = simulationBuilder.CreateSimulation(parameters);
                var results = await Task.Run(() => simulation.Run());

                var resultsJson = _simulationHandler.ToJson(results);
                await FileHelper.WriteFileAsync(FileHelper.SimulationResultsPath, resultsJson);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                _hostApplicationLifetime.StopApplication();
            }
        }
    }
}