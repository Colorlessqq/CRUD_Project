using GameStore.Application.Dtos;
using GameStore.Application.Interfaces;

namespace GameStore.Application.Services;

public class GenreService : IGenreService
{
    private readonly IGenreRepository _repository;

    public GenreService(IGenreRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<GenreDto>> GetAllGenresAsync()
    {
        var genres = await _repository.GetAllAsync();
        
        // Map the Domain Entities to DTOs for the API
        return genres.Select(genre => new GenreDto(
            genre.Id,
            genre.Name
        ));
    }
}