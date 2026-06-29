using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace LostPeople.Infrastructure.Persistence;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<LostPeopleDbContext>
{
    public LostPeopleDbContext CreateDbContext(string[] args)
    {
        var server = Environment.GetEnvironmentVariable("LOSTPEOPLE_DB_SERVER") ?? "localhost";
        var database = Environment.GetEnvironmentVariable("LOSTPEOPLE_DB_NAME") ?? "LostPeople";
        var user = Environment.GetEnvironmentVariable("LOSTPEOPLE_DB_USER") ?? "sa";
        var password = Environment.GetEnvironmentVariable("LOSTPEOPLE_DB_PASSWORD") ?? "";
        var connStr = $"Server={server};Database={database};User Id={user};Password={password};TrustServerCertificate=True;Connection Timeout=30;";

        var optionsBuilder = new DbContextOptionsBuilder<LostPeopleDbContext>();
        optionsBuilder.UseSqlServer(connStr);
        return new LostPeopleDbContext(optionsBuilder.Options);
    }
}