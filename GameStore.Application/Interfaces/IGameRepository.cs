using GameStore.Domain.Models;
using GameStore.Application.Dtos;

namespace GameStore.Application.Interfaces;

public interface IGameRepository
{
    Task<IEnumerable<GameSummaryDto>> GetAllAsync();
    Task<GameDetailsDto?> GetByIdAsync(string id);
    Task<Game> AddAsync(Game game);
    Task<bool> UpdateAsync(string id, Game updatedGame);
    Task<bool> DeleteAsync(string id);
}