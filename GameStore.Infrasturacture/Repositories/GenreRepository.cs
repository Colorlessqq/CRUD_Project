using GameStore.Application.Interfaces;
using GameStore.Domain.Models;
using GameStore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Infrastructure.Repositories;

public class GenreRepository : IGenreRepository
{
    private readonly GameStoreContext _dbContext;

    public GenreRepository(GameStoreContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Genre>> GetAllAsync()
    {
        return await _dbContext.Genres
            .AsNoTracking()
            .ToListAsync();
    }
}