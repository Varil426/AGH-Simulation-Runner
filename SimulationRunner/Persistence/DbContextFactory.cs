/*using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Persistence;

public class DbContextFactory : IDesignTimeDbContextFactory<DataContext>
{
    public DataContext CreateDbContext(string[] args)
    {
        Console.WriteLine(Directory.GetCurrentDirectory());

        var configuration = new ConfigurationBuilder()
            //.SetBasePath(Directory.GetCurrentDirectory())
            //.AddJsonFile("../BackendAPI/appsettings.Development.json") // TODO Change to different file in production
            .Build();

        var dbContextBuilder = new DbContextOptionsBuilder<DataContext>();

        var connectionString = "User ID=root;Password=root;Host=postgresHost;Port=5432;Database=SimulationRunner;Pooling=true";

        dbContextBuilder.UseNpgsql(connectionString);

        return new DataContext(dbContextBuilder.Options);
    }
}*/