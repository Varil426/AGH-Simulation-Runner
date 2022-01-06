using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;

namespace Application.Docker;

internal static class FileHelper
{
    private static readonly string _tarFileExtension = "tar.gz";

    /// <summary>
    /// Packs directory into tar file.
    /// </summary>
    /// <param name="directoryPath">Path to a directory.</param>
    /// <returns>Path to newly created tar file.</returns>
    /// <exception cref="ArgumentException">If nondirectory path was passed.</exception>
    public static async Task<string> CreateTarArchive(string directoryPath)
    {
        var attributes = File.GetAttributes(directoryPath);
        if (!attributes.HasFlag(FileAttributes.Directory))
            throw new ArgumentException("Expected path to directory.");

        var outputFilePath = Path.ChangeExtension(directoryPath, _tarFileExtension);
        using var outStream = File.Create(outputFilePath);
        using var gzoStream = new GZipOutputStream(outStream);
        using var tarArchive = TarArchive.CreateOutputTarArchive(gzoStream);

        var entries = new List<TarEntry>();
        AddAllFilesRecursively(entries, directoryPath, directoryPath);

        foreach (var entry in entries)
        {
            await Task.Run(() => tarArchive.WriteEntry(entry, false));
        }

        return outputFilePath;
    }

    private static void AddAllFilesRecursively(List<TarEntry> tarEntries, string currentPath, string rootPath)
    {
        foreach (var filePath in Directory.GetFiles(currentPath))
        {
            var entry = TarEntry.CreateEntryFromFile(filePath);
            entry.Name = Path.GetRelativePath(rootPath, filePath);
            tarEntries.Add(entry);
        }

        foreach (var directoryPath in Directory.GetDirectories(currentPath))
            AddAllFilesRecursively(tarEntries, directoryPath, rootPath);
    }
}