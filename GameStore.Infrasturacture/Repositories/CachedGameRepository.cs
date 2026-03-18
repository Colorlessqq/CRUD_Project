using System.Text.Json;
using GameStore.Application.Dtos;
using GameStore.Application.Interfaces;
using GameStore.Domain.Models;
using Microsoft.Extensions.Caching.Distributed;

namespace GameStore.Infrastructure.Repositories;

public class CachedGameRepository : IGameRepository
{
    private readonly IGameRepository _decorated;
    private readonly IDistributedCache _cache;

    public CachedGameRepository(IGameRepository decorated, IDistributedCache cache)
    {
        _decorated = decorated;
        _cache = cache;
    }

    public async Task<IEnumerable<GameSummaryDto>> GetAllAsync()
    {
        string cacheKey = "all_games_list";
        var cachedGames = await _cache.GetStringAsync(cacheKey);

        if (!string.IsNullOrEmpty(cachedGames))
        {
            Console.WriteLine("Bouncer: Found games on clipboard! ⚡");
            return JsonSerializer.Deserialize<IEnumerable<GameSummaryDto>>(cachedGames)!;
        }

        Console.WriteLine("Bouncer: Going into the club... 🐌");
        var games = await _decorated.GetAllAsync();

        var cacheOptions = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2) };
        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(games), cacheOptions);

        return games;
    }

    public async Task<GameDetailsDto?> GetByIdAsync(string id)
    {
        string cacheKey = $"game_{id}";
        var cachedGame = await _cache.GetStringAsync(cacheKey);

        if (!string.IsNullOrEmpty(cachedGame))
        {
            Console.WriteLine($"Bouncer: Found game {id} on clipboard! ⚡");
            return JsonSerializer.Deserialize<GameDetailsDto>(cachedGame);
        }

        Console.WriteLine($"Bouncer: Going into the club for game {id}... 🐌");
        var game = await _decorated.GetByIdAsync(id);

        if (game is null) return null;

        var cacheOptions = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2) };
        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(game), cacheOptions);

        return game;
    }

    public async Task<Game> AddAsync(Game game)
    {
        var createdGame = await _decorated.AddAsync(game);
        
        // Destroy master list cache
        await _cache.RemoveAsync("all_games_list");
        
        return createdGame;
    }

    public async Task<bool> UpdateAsync(string id, Game updatedGame)
    {
        bool isUpdated = await _decorated.UpdateAsync(id, updatedGame);

        if (isUpdated)
        {
            // Destroy BOTH caches!
            await _cache.RemoveAsync("all_games_list");
            await _cache.RemoveAsync($"game_{id}");
        }

        return isUpdated;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        bool isDeleted = await _decorated.DeleteAsync(id);

        if (isDeleted)
        {
            // Destroy BOTH caches!
            await _cache.RemoveAsync("all_games_list");
            await _cache.RemoveAsync($"game_{id}");
        }

        return isDeleted;
    }
}