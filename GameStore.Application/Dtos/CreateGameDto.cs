namespace GameStore.Application.Dtos;

public record class CreateGameDto
(
    string Name,
    string GenreId,
    decimal Price,
    DateOnly ReleaseDate
);