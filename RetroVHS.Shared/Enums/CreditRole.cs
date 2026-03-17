namespace RetroVHS.Shared.Enums;

/// <summary>
/// Describes what role a person had in a movie.
/// This allows the same person to be connected to a movie
/// as for example Actor, Director, Writer, etc.
/// </summary>
public enum CreditRole
{
    Actor = 1,
    Director = 2,
    Writer = 3,
    Producer = 4
}