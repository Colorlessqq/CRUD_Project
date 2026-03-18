using GameStore.Application.Interfaces;
using GameStore.Domain.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace GameStore.Infrastructure.Repositories;

public class MongoGenreRepository : IGenreRepository
{
    private readonly IMongoCollection<Genre> _genresCollection;

    public MongoGenreRepository(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("MongoDb");
        var databaseName = configuration["MongoDbSettings:DatabaseName"];

        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);
        _genresCollection = database.GetCollection<Genre>("Genres");
    }

    public async Task<IEnumerable<Genre>> GetAllAsync()
    {
        // Find(_ => true) gets all genres
        return await _genresCollection.Find(_ => true).ToListAsync();
    }
}