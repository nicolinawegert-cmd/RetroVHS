namespace RetroVHS.Shared.DTOs.Movies;

public class GenreSectionDto
{
    public int GenreId { get; set; }
    public string GenreName { get; set; } = string.Empty;
    public List<MovieListDto> Movies { get; set; } = [];
}
