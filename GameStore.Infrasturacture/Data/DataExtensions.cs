using GameStore.Domain.Models; // <-- Needed for Genre seeding
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GameStore.Infrastructure.Data; // <-- Updated namespace

public static class DataExtensions
{
    // We use IServiceProvider instead of WebApplication. 
    // This makes the method usable in any project type (Web, Console, Worker Service).
    public static void MigrateDb(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<GameStoreContext>();
        dbContext.Database.Migrate();
    }

    // We use IServiceCollection and IConfiguration instead of WebApplicationBuilder.
    public static IServiceCollection AddGameStoreInfrastructure(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        var connString = configuration.GetConnectionString("GameStore");
        
        services.AddSqlite<GameStoreContext>(connString, optionsAction: options => 
            options.UseSeeding((context, _) =>
            {
                if (!context.Set<Genre>().Any())
                {
                    context.Set<Genre>().AddRange(
                        new Genre { Name = "Fighting0" },
                        new Genre { Name = "Fighting1" },
                        new Genre { Name = "Fighting2" },
                        new Genre { Name = "Fighting3" },
                        new Genre { Name = "Fighting4" }
                    );
                    context.SaveChanges();
                }
            }));

        return services;
    }
}