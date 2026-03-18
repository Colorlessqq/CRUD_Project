using System;
using System.Text.Json;
using GameStore.Application.Dtos;
using GameStore.Application.Interfaces;
using GameStore.Domain.Models;
using Microsoft.Extensions.Caching.Distributed;

namespace GameStore.Infrastructure.Repositories;

public class CachedGenreRepository : IGenreRepository
{
    private readonly IGenreRepository _decorated;
    private readonly IDistributedCache _cache;
    public CachedGenreRepository(IGenreRepository genreRepository, IDistributedCache cache)
    {
        _decorated = genreRepository;
        _cache = cache;
    }
    public async Task<IEnumerable<Genre>> GetAllAsync()
    {
        string cacheKey = "all_genres_list";
        var cachedGames = await _cache.GetStringAsync(cacheKey);

        if (!string.IsNullOrEmpty(cachedGames))
        {
            Console.WriteLine("Bouncer: Found genres on clipboard! ⚡");
            return JsonSerializer.Deserialize<IEnumerable<Genre>>(cachedGames)!;
        }

        Console.WriteLine("Bouncer: Going into the club... 🐌");
        var genres = await _decorated.GetAllAsync();

        var cacheOptions = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2) };
        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(genres), cacheOptions);

        return genres;
    }

}
