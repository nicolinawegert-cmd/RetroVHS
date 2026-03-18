namespace RetroVHS.Shared.DTOs.Movies;

/// <summary>
/// DTO som beskriver en person kopplad till en film,
/// t.ex. en skådespelare eller regissör.
/// Vi använder denna i filmens detaljvy.
/// </summary>
public class PersonCreditDto
{
    /// <summary>
    /// Personens id
    /// </summary>
    public int PersonId { get; set; }

    /// <summary>
    /// Personens fullständiga namn
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Personens roll i filmen, t.ex. Actor eller Director
    /// </summary>
    public string Role { get; set; } = string.Empty;

    /// <summary>
    /// Namnet på karaktären personen spelar, om rollen är skådespelare
    /// </summary>
    public string? CharacterName { get; set; }

    /// <summary>
    /// Ordningen vi vill visa credits i
    /// </summary>
    public int DisplayOrder { get; set; }
}