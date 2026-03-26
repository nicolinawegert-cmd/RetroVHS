using RetroVHS.Shared.Enums;

namespace RetroVHS.Client.Helpers;

/// <summary>
/// Gemensam hjälpklass för att omvandla RentalStatus-enum till CSS-klass och visningstext.
/// Används av både MyOrders.razor (användarvy) och Admin/UserOrders.razor (adminvy)
/// för att hålla statusvisningen konsekvent.
/// </summary>
public static class RentalStatusHelper
{
    // Returnerar CSS-klassnamn som matchar stilar i app.css (t.ex. "status-active")
    public static string GetStatusClass(RentalStatus s) => s switch
    {
        RentalStatus.Active    => "status-active",
        RentalStatus.Completed => "status-completed",
        RentalStatus.Cancelled => "status-cancelled",
        RentalStatus.Expired   => "status-expired",
        _                      => ""
    };

    public static string GetStatusLabel(RentalStatus s) => s switch
    {
        RentalStatus.Active    => "Aktiv",
        RentalStatus.Completed => "Levererad",
        RentalStatus.Cancelled => "Avbruten",
        RentalStatus.Expired   => "Utgången",
        _                      => s.ToString()
    };
}
