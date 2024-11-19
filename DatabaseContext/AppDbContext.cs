using Microsoft.EntityFrameworkCore;
using MovieAPI.Model;
namespace MovieAPI.DatabaseContext;
public class AppDbContext : DbContext
{
    public DbSet<Movies> Movies { get; set; }
    public DbSet<TVShows> TVShows { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        optionsBuilder.UseSqlite(configuration.GetConnectionString("DefaultConnection"));
    }
}
