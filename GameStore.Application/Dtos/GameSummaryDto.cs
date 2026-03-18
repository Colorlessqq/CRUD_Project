namespace GameStore.Application.Dtos;

public record class GameSummaryDto
(
    string Id,
    string Name,
    string Genre,
    decimal Price,
    DateOnly ReleaseDate
);