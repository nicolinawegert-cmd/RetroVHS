namespace RetroVHS.Client.Services;

/// <summary>
/// Delad state-tjänst för varukorgen.
/// Används för att meddela Header-komponenten när varukorgen ändras
/// så att räknaren kan uppdateras utan att ladda om sidan.
/// </summary>
public class CartState
{
    /// <summary>
    /// Antal filmer i varukorgen just nu.
    /// </summary>
    public int ItemCount { get; private set; }

    /// <summary>
    /// Event som triggas när varukorgen ändras.
    /// Header-komponenten prenumererar på detta.
    /// </summary>
    public event Action? OnChange;

    /// <summary>
    /// Uppdaterar antalet och meddelar alla lyssnare.
    /// </summary>
    public void SetCount(int count)
    {
        ItemCount = count;
        OnChange?.Invoke();
    }
}