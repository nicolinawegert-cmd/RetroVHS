namespace RetroVHS.Shared.Enums;

/// <summary>
/// Beskriver status för en varukorg.
/// Vi använder enum för att göra koden tydligare och undvika hårdkodade strängar.
/// </summary>
public enum CartStatus
{
    Active = 1,
    CheckedOut = 2,
    Abandoned = 3
}