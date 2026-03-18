namespace GameStore.Application.Dtos;

public record class GameDetailsDto
(
    string Id,
    string Name,
    string GenreId,
    decimal Price,
    DateOnly ReleaseDate
);