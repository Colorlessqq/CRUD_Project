using GameStore.Application.Dtos;
using GameStore.Application.Interfaces;

namespace GameStore.Api.EndPoints;

public static class GamesEndpoints
{
    const string GamePointEndpointName = "GetGame";
    
    public static void MapGamesEndPoints(this WebApplication app)
    {
        app.MapGet("/", () => "Hello World!");

        app.MapGet("/games", async (IGameService gameService) => 
            await gameService.GetAllGamesAsync());

        app.MapGet("/games/{id}", async (string id, IGameService gameService) =>
        {
            var game = await gameService.GetGameByIdAsync(id);
            return game is null ? Results.NotFound() : Results.Ok(game);
        }).WithName(GamePointEndpointName);

        app.MapPost("/games", async (CreateGameDto newGame, IGameService gameService) =>
        {
            var gameDto = await gameService.CreateGameAsync(newGame);
            return Results.CreatedAtRoute(GamePointEndpointName, new { id = gameDto.Id }, gameDto);
        });

        app.MapPut("/games/{id}", async (string id, UpdateGameDto updatedGame, IGameService gameService) =>
        {
            var updated = await gameService.UpdateGameAsync(id, updatedGame);
            return updated ? Results.NoContent() : Results.NotFound();
        });

        app.MapDelete("/games/{id}", async (string id, IGameService gameService) =>
        {
            await gameService.DeleteGameAsync(id);
            return Results.NoContent();
        });
    }
}