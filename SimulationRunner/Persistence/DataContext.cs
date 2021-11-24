using Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Persistence;
public class DataContext : IdentityDbContext<User>
{
    public DataContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Simulation> Simulations { get; set; } = null!;

    public DbSet<SimulationRunAttempt> RunAttempts { get; set; } = null!;

    public DbSet<SimulationParamTemplate> ParamTemplates { get; set; } = null!;

    public DbSet<SimulationResultTemplate> ResultTemplates { get; set; } = null!;

    public DbSet<ParamValue> ParamValues { get; set; } = null!;

    public DbSet<ResultValue> ResultValues { get; set; } = null!;

    public DbSet<ValueOfParamCollection> ValuesOfParamCollections { get; set; } = null!;

    public DbSet<ValueOfResultCollection> ValuesOfResultCollections { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Simulation>().Property(x => x.FileType)
            //.HasConversion(x => x.ToString(), x => (Simulation.AllowedFileTypesEnum)Enum.Parse(typeof(Simulation.AllowedFileTypesEnum), x));
            .HasConversion<string>();
    }
}