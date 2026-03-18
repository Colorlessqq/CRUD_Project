using GameStore.Application.Dtos;
using GameStore.Application.Interfaces; // Ensure this matches your interface's namespace
using GameStore.Domain.Models; // Ensure this matches where your Game entity lives

namespace GameStore.Application.Services;

public class GameService : IGameService
{
    private readonly IGameRepository _repository;
    private readonly IMessagePublisher _messagePublisher;

    // Inject the interface, NOT the database context
    public GameService(IGameRepository repository, IMessagePublisher messagePublisher)
    {
        _repository = repository;
        _messagePublisher = messagePublisher;
    }

    public async Task<IEnumerable<GameSummaryDto>> GetAllGamesAsync()
    {
        // The repository handles the database fetching
        return await _repository.GetAllAsync();
    }

    public async Task<GameDetailsDto?> GetGameByIdAsync(string id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<GameDetailsDto> CreateGameAsync(CreateGameDto newGame)
    {
        // 1. Map the DTO to the Domain Entity
        Game game = new()
        {
            Name = newGame.Name,  
            GenreId = newGame.GenreId,
            Price = newGame.Price,
            ReleaseDate = newGame.ReleaseDate
        };

        // 2. Pass the Entity to the repository to be saved
        var createdGame = await _repository.AddAsync(game);

        var gameDto = new GameDetailsDto(
            createdGame.Id,
            createdGame.Name,
            createdGame.GenreId,
            createdGame.Price,
            createdGame.ReleaseDate
        );

        // THE NEW MAGIC LINE: Drop a ticket in the RabbitMQ window!
        // We name our mailbox "game_updates"
        await _messagePublisher.PublishMessageAsync(gameDto, "game_updates");

        return gameDto;
    }

    public async Task<bool> UpdateGameAsync(string id, UpdateGameDto updatedGame)
    {
        // Map the DTO to a Domain Entity
        Game game = new()
        {
            Id = id,
            Name = updatedGame.Name,
            GenreId = updatedGame.GenreId,
            Price = updatedGame.Price,
            ReleaseDate = updatedGame.ReleaseDate
        };

        // Pass it to the repository to update
        return await _repository.UpdateAsync(id, game);
    }

    public async Task<bool> DeleteGameAsync(string id)
    {
        return await _repository.DeleteAsync(id);
    }
}