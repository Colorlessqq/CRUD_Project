using GameStore.Application.Interfaces;

namespace GameStore.Api.EndPoints;

public static class GenresEndpoints
{
    public static void MapGenresEndpoint(this WebApplication app)
    {
        app.MapGet("/genres", async (IGenreService genreService) => 
            await genreService.GetAllGenresAsync());
    }
}