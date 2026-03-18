using GameStore.Application.Dtos;
using GameStore.Application.Interfaces;
using GameStore.Domain.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace GameStore.Infrastructure.Repositories;

public class MongoGameRepository : IGameRepository
{
    private readonly IMongoCollection<Game> _gamesCollection;
    // Read from appsettings.json
    private readonly IMongoCollection<Genre> _genresCollection;

    public MongoGameRepository(IConfiguration configuration)
    {
        // Read from appsettings.json
        var connectionString = configuration.GetConnectionString("MongoDb");
        var databaseName = configuration["MongoDbSettings:DatabaseName"];

        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);
        
        _gamesCollection = database.GetCollection<Game>("Games");
        _genresCollection = database.GetCollection<Genre>("Genres");
    }
    public async Task<IEnumerable<GameSummaryDto>> GetAllAsync()
    {
        // 2. Fetch the games
        var games = await _gamesCollection.Find(_ => true).ToListAsync();
        
        // 3. Fetch the genres (Since there are only a few genres, fetching them is extremely fast)
        var genres = await _genresCollection.Find(_ => true).ToListAsync();
        
        // 4. Stitch them together using C# LINQ
        return games.Select(game => 
        {
            // Look up the matching genre for this specific game
            var matchedGenre = genres.FirstOrDefault(g => g.Id == game.GenreId);

            return new GameSummaryDto(
                game.Id,
                game.Name,
                matchedGenre?.Name ?? "Unknown Genre", // If it finds the genre, use the name!
                game.Price,
                game.ReleaseDate
            );
        });
    }

    public async Task<GameDetailsDto?> GetByIdAsync(string id)
    {
        var game = await _gamesCollection.Find(g => g.Id == id).FirstOrDefaultAsync();

        if (game is null) return null;

        // GameDetailsDto only asks for GenreId, so we don't need to look up the name here
        return new GameDetailsDto(
            game.Id, game.Name, game.GenreId, game.Price, game.ReleaseDate
        );
    }

    public async Task<Game> AddAsync(Game game)
    {
        game.Id = Guid.NewGuid().ToString(); 
        await _gamesCollection.InsertOneAsync(game);
        return game;
    }

    public async Task<bool> UpdateAsync(string id, Game updatedGame)
    {
        updatedGame.Id = id; 
        var result = await _gamesCollection.ReplaceOneAsync(g => g.Id == id, updatedGame);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _gamesCollection.DeleteOneAsync(g => g.Id == id);
        return result.DeletedCount > 0;
    }
}