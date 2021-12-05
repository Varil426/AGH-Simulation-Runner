using SimulationRunnerService;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
        services.AddTransient<ISimulationHandler, SimulationHandler.SimulationHandler>();
    })
    .Build();

await host.RunAsync();
