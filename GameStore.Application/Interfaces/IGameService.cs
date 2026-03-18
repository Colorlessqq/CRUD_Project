using GameStore.Application.Dtos;

namespace GameStore.Application.Interfaces;

public interface IGameService
{
    Task<IEnumerable<GameSummaryDto>> GetAllGamesAsync();
    Task<GameDetailsDto?> GetGameByIdAsync(string id);
    Task<GameDetailsDto> CreateGameAsync(CreateGameDto newGame);
    Task<bool> UpdateGameAsync(string id, UpdateGameDto updatedGame);
    Task<bool> DeleteGameAsync(string id);
}