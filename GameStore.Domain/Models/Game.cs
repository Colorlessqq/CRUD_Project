namespace GameStore.Domain.Models;

public class Game
{
    // Not required, because the database creates it later!
    public string Id { get; set; } = string.Empty; 
    
    // Required, because a game MUST have a name and genre when created
    public required string Name { get; set; } 
    public required string GenreId { get; set; } 
    
    public decimal Price { get; set; }
    public DateOnly ReleaseDate { get; set; }
    public Genre? Genre { get; set; }
}