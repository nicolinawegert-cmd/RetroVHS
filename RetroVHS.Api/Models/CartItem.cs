using System.ComponentModel.DataAnnotations;

namespace RetroVHS.Api.Models;

/// <summary>
/// Representerar en film som ligger i en varukorg.
///
/// Vi använder en separat modell för att koppla ihop varukorg och film.
/// Det gör att en varukorg kan innehålla flera filmer och att vi kan spara
/// information om priset vid tillfället då filmen lades i varukorgen.
/// </summary>
public class CartItem
{
    /// <summary>
    /// Primärnyckel för raden i varukorgen
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Främmande nyckel till varukorgen
    /// </summary>
    public int CartId { get; set; }

    /// <summary>
    /// Navigation till varukorgen
    /// </summary>
    public Cart Cart { get; set; } = null!;

    /// <summary>
    /// Främmande nyckel till filmen
    /// </summary>
    public int MovieId { get; set; }

    /// <summary>
    /// Navigation till filmen
    /// </summary>
    public Movie Movie { get; set; } = null!;

    /// <summary>
    /// Priset för filmen när den lades i varukorgen.
    /// Vi sparar detta separat för att historiken i varukorgen inte ska ändras
    /// om filmens ordinarie pris ändras senare.
    /// </summary>
    [Range(typeof(decimal), "0.01", "999.99")]
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Antal exemplar av filmen i varukorgen.
    /// För en digital hyrtjänst kommer detta normalt vara 1,
    /// men vi har med fältet för att modellen ska vara tydlig och flexibel.
    /// </summary>
    [Range(1, 1)]
    public int Quantity { get; set; } = 1;

    /// <summary>
    /// Tidpunkt då filmen lades till i varukorgen
    /// </summary>
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}