using RetroVHS.Shared.Enums;

namespace RetroVHS.Api.Models;

/// <summary>
/// Representerar en varukorg för en användare.
///
/// Vi använder en separat modell för varukorgen för att kunna samla flera filmer
/// innan uthyrning genomförs. Det gör checkout-flödet tydligare och mer realistiskt.
/// </summary>
public class Cart
{
    /// <summary>
    /// Primärnyckel för varukorgen
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Främmande nyckel till användaren som äger varukorgen
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Navigation till användaren som äger varukorgen
    /// </summary>
    public ApplicationUser User { get; set; } = null!;

    /// <summary>
    /// Status för varukorgen.
    /// Exempel: aktiv, utcheckad eller övergiven.
    /// </summary>
    public CartStatus Status { get; set; } = CartStatus.Active;

    /// <summary>
    /// Tidpunkt då varukorgen skapades
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Tidpunkt då varukorgen senast uppdaterades
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    // =========================
    // Navigation properties
    // =========================

    /// <summary>
    /// Alla filmer som ligger i varukorgen via CartItem.
    /// Vi använder en separat tabell eftersom en varukorg kan innehålla flera filmer.
    /// </summary>
    public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
}