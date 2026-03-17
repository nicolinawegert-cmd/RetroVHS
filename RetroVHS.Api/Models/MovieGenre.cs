namespace RetroVHS.Api.Models;

/// <summary>
/// Join-tabell mellan Movie och Genre.
/// 
/// Vi använder denna eftersom relationen mellan filmer och genrer är many-to-many:
/// en film kan ha flera genrer och en genre kan finnas på flera filmer.
/// 
/// Den här tabellen innehåller därför främmande nycklar till båda tabellerna.
/// </summary>
public class MovieGenre
{
    /// <summary>
    /// Främmande nyckel till filmen
    /// </summary>
    public int MovieId { get; set; }

    /// <summary>
    /// Navigation till filmen
    /// </summary>
    public Movie Movie { get; set; } = null!;

    /// <summary>
    /// Främmande nyckel till genren
    /// </summary>
    public int GenreId { get; set; }

    /// <summary>
    /// Navigation till genren
    /// </summary>
    public Genre Genre { get; set; } = null!;
}

//Eftersom en film kan ha flera genrer
//en genre kan tillhöra flera filmer
//så behöver vi en mellanliggande tabell.
//Det är exakt vad MovieGenre är.