using GameStore.Domain.Models; // <-- Make sure your Models are in the Domain project!
using Microsoft.EntityFrameworkCore;

namespace GameStore.Infrastructure.Data; // <-- Updated namespace

public class GameStoreContext(DbContextOptions<GameStoreContext> options) : DbContext(options)
{
    public DbSet<Game> Games => Set<Game>();
    public DbSet<Genre> Genres => Set<Genre>();
}