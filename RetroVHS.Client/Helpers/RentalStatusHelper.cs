using RetroVHS.Shared.Enums;

namespace RetroVHS.Client.Helpers;

public static class RentalStatusHelper
{
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
