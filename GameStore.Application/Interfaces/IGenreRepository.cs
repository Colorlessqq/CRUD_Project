using GameStore.Domain.Models;

namespace GameStore.Application.Interfaces;

public interface IGenreRepository
{
    Task<IEnumerable<Genre>> GetAllAsync();
}