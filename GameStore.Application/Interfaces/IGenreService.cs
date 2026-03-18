using GameStore.Application.Dtos;

namespace GameStore.Application.Interfaces;

public interface IGenreService
{
    Task<IEnumerable<GenreDto>> GetAllGenresAsync();
}