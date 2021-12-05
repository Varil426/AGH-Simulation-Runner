namespace SimulationRunnerService;

internal static class FileHelper
{
    public static string DataDirectoryPath => Path.GetFullPath(Environment.GetEnvironmentVariable("SIMULATION_RUNNER_SERVICE_DATA_PATH") ?? "data");

    // TODO Improve to detect whether it's .dll or .zip
    public static string SimulationFilesPath => Path.ChangeExtension(Path.Combine(DataDirectoryPath, ISimulationHandler.SimulationFileFileName), "dll");

    public static string SimulationParametersTemplatePath => Path.ChangeExtension(Path.Combine(DataDirectoryPath, ISimulationHandler.SimulationParametersTemplateFileName), ISimulationHandler.JsonFileExtension.ToLower());
    public static string SimulationResultsTemplatePath => Path.ChangeExtension(Path.Combine(DataDirectoryPath, ISimulationHandler.SimulationResultsTemplateFileName), ISimulationHandler.JsonFileExtension.ToLower());
    public static string SimulationParametersPath => Path.ChangeExtension(Path.Combine(DataDirectoryPath, ISimulationHandler.SimulationParametersFileName), ISimulationHandler.JsonFileExtension.ToLower());
    public static string SimulationResultsPath => Path.ChangeExtension(Path.Combine(DataDirectoryPath, ISimulationHandler.SimulationResultsFileName), ISimulationHandler.JsonFileExtension.ToLower());

    public static async Task<string> ReadFileAsync(string filePath)
    {
        using var fileStream = new FileStream(filePath, FileMode.Open);
        using var streamReader = new StreamReader(fileStream);
        return await streamReader.ReadToEndAsync();
    }

    public static async Task WriteFileAsync(string filePath, string content)
    {
        using var resultsFileStream = new FileStream(filePath, FileMode.OpenOrCreate);
        using var resultsStreamWriter = new StreamWriter(resultsFileStream);
        await resultsStreamWriter.WriteAsync(content);
    }

    /// <summary>
    /// Checks directory structure. Looks for all files necessary to run a simulation.
    /// </summary>
    /// <returns><see langword="true"/> if every needed file is present; <see langword="false"/> otherwise.</returns>
    internal static bool CheckDirectoryStructure() => Directory.Exists(DataDirectoryPath) && File.Exists(SimulationFilesPath) && File.Exists(SimulationResultsTemplatePath) && File.Exists(SimulationParametersPath); // TODO Templates are not necessary anymore
}