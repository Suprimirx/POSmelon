using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DrogueriaPOS.Infrastructure.Context;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        var appDataFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "DrogueriaPOS");

        Directory.CreateDirectory(appDataFolder);

        var dbPath = Path.Combine(appDataFolder, "drogueria.db");
        optionsBuilder.UseSqlite($"Data Source={dbPath}");
        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
