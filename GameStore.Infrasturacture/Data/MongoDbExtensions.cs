using GameStore.Domain.Models;
using MongoDB.Driver;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace GameStore.Infrastructure.Data;

public static class MongoDbExtensions
{
    public static void SeedMongoDb(this IServiceProvider serviceProvider)
    {
        // Grab the configuration from the app's services
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        var connectionString = configuration.GetConnectionString("MongoDb");
        var databaseName = configuration["MongoDbSettings:DatabaseName"];
        
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);
        var genresCollection = database.GetCollection<Genre>("Genres");
        // 2. Check if it's already seeded (so we don't add duplicates!)
        var existingCount = genresCollection.CountDocuments(FilterDefinition<Genre>.Empty);
        
        // Inside your SeedMongoDb method:

        if (existingCount == 0)
        {
            var genres = new List<Genre>
            {
                new Genre { Id = "1", Name = "Fighting" },
                new Genre { Id = "2", Name = "Roleplaying" },
                new Genre { Id = "3", Name = "Sports" },
                new Genre { Id = "4", Name = "Racing" },
                new Genre { Id = "5", Name = "Kids and Family" }
            };

            // 4. Insert them into MongoDB
            genresCollection.InsertMany(genres);
        }
    }
}