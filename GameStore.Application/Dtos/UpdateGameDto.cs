namespace GameStore.Application.Dtos;

public record UpdateGameDto
(
    string Name,
    string GenreId,
    decimal Price,
    DateOnly ReleaseDate
);