using Microsoft.EntityFrameworkCore;
using RetroVHS.Api.Data;
using RetroVHS.Api.Models;
using RetroVHS.Shared.DTOs.Cart;
using RetroVHS.Shared.DTOs.Rentals;
using RetroVHS.Shared.Enums;

namespace RetroVHS.Api.Services.Cart
{
    public class CartService : ICartService
    {
        /// Vi skriver Models.Cart i våra metoder nedan för att nå vår entity 
        /// så det inte krockar med vår namespace här i CartService som också heter Cart.
        /// 
      
        private readonly ApplicationDbContext _db;

        /// <summary>
        /// Konstruktor som tar emot databaskontexten via dependency injection.
        /// </summary>
        public CartService(ApplicationDbContext db)
        {
            _db = db;
        }


        /// <summary>
        /// Lägger till en film i användarens varukorg.
        /// Kontrollerar att filmen inte redan finns i varukorgen
        /// och hämtar aktuellt pris från databasen.
        /// </summary>
        public async Task<CartDto> AddToCartAsync(int userId, AddToCartDto dto)
        {
            // Hämta eller skapa en aktiv varukorg.
            var cart = await GetOrCreateActiveCartAsync(userId);

            // Hämta filmen från databasen för att få aktuellt pris
            var movie = await _db.Movies.FindAsync(dto.MovieId)
                ?? throw new ArgumentException("Filmen hittades inte.");  //Om filmen inte finns i databasen, kasta ett undantag

            if (movie.StockQuantity <= 0)
                throw new InvalidOperationException("Filmen är slut i lager.");

            // Om filmen redan finns i varukorgen, öka antalet istället
            var existingItem = cart.Items.FirstOrDefault(i => i.MovieId == dto.MovieId);
            if (existingItem != null)
            {
                if (existingItem.Quantity >= movie.StockQuantity)
                    throw new InvalidOperationException("Du kan inte lägga till fler exemplar än vad som finns i lager.");

                existingItem.Quantity++;
                cart.UpdatedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();
                return MapToCartDto(cart);
            }

            // Skapa en ny rad i varukorgen
            var cartItem = new CartItem
            {
                CartId = cart.Id,
                MovieId = movie.Id,
                UnitPrice = movie.RentalPrice,
                Quantity = dto.Quantity
            };

            cart.Items.Add(cartItem);
            cart.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            // Returnera uppdaterad varukorg till klienten
            return MapToCartDto(cart);
        }



        /// <summary>
        /// Genomför checkout: skapar en Rental-post för varje film i varukorgen,
        /// markerar varukorgen som utcheckad och returnerar en bekräftelse.
        /// </summary>
        public async Task<CheckoutResponseDto> CheckoutAsync(int userId, CheckoutDto dto)
        {
            // Hämta varukorgen med alla items och filmdata
            var cart = await _db.Carts
                .Include(c => c.Items)
                    .ThenInclude(i => i.Movie)
                .FirstOrDefaultAsync(c => c.Id == dto.CartId
                                       && c.UserId == userId
                                       && c.Status == CartStatus.Active);

            // Kontrollera att varukorgen finns och inte är tom
            if (cart == null || cart.Items.Count == 0)
            {
                return new CheckoutResponseDto
                {
                    Success = false,
                    Message = "Varukorgen är tom eller hittades inte."
                };
            }

            // Kontrollera lagersaldo för varje film INNAN köpet genomförs
            var outOfStockTitles = cart.Items
                .Where(i => i.Movie.StockQuantity <= 0)
                .Select(i => i.Movie.Title)
                .ToList();

            if (outOfStockTitles.Count > 0)
            {
                return new CheckoutResponseDto
                {
                    Success = false,
                    Message = $"Följande filmer är slut i lager: {string.Join(", ", outOfStockTitles)}"
                };
            }

            // Skapa en Rental-post för varje film i varukorgen
            var rentals = new List<Rental>();
            foreach (var item in cart.Items)
            {
                var rental = new Rental
                {
                    UserId = userId,
                    MovieId = item.MovieId,
                    PricePaid = item.UnitPrice,
                    RentedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.MaxValue,          // Köpt film – ingen utgångstid
                    Status = RentalStatus.Active,
                    PaymentReference = $"SIM-{Guid.NewGuid():N}"[..16]
                };
                rentals.Add(rental);


                // Uppdatera lagersaldo för filmen
                item.Movie.StockQuantity--;             // Minska lagersaldo med 1 för varje uthyrd film
                if (item.Movie.StockQuantity <= 0)      // Om lagersaldo är 0 eller mindre, markera filmen som slut
                    item.Movie.AvailabilityStatus = MovieAvailabilityStatus.OutOfStock;
            }

            _db.Rentals.AddRange(rentals);  // Lägg till alla nya rentals i databasen

            // Markera varukorgen som utcheckad
            cart.Status = CartStatus.CheckedOut;
            cart.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            // Returnera bekräftelse till klienten
            return new CheckoutResponseDto
            {
                Success = true,
                Message = $"Köpet genomfört! Du köpte {rentals.Count} film(er).",
                Rentals = rentals.Select(r => new RentalDto
                {
                    Id = r.Id,
                    MovieId = r.MovieId,
                    Title = cart.Items.First(i => i.MovieId == r.MovieId).Movie.Title,
                    PricePaid = r.PricePaid,
                    RentedAt = r.RentedAt,
                    ExpiresAt = r.ExpiresAt,
                    Status = r.Status
                }).ToList()
            };
        }



        /// <summary>
        /// Hämtar den aktiva varukorgen för en användare.
        /// Om ingen aktiv varukorg finns skapas en ny automatiskt.
        /// </summary>
        public async Task<CartDto> GetCartAsync(int userId)
        {
            var cart = await GetOrCreateActiveCartAsync(userId);
            return MapToCartDto(cart);
        }


        /// <summary>
        /// Tar bort en specifik film ur varukorgen.
        /// Returnerar true om borttagningen lyckades, false om raden inte hittades.
        /// </summary>
        public async Task<bool> RemoveFromCartAsync(int userId, int cartItemId)
        {
            var cart = await GetOrCreateActiveCartAsync(userId);

            // Hitta raden som ska tas bort
            var item = cart.Items.FirstOrDefault(i => i.Id == cartItemId);
            if (item == null)
                return false;

            _db.CartItems.Remove(item);
            cart.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            return true;
        }


        /// <summary>
        /// Hämtar användarens aktiva varukorg från databasen.
        /// Om ingen aktiv varukorg finns skapas en ny automatiskt.
        /// Denna metod används internt av de publika metoderna.
        /// </summary>
        private async Task<Models.Cart> GetOrCreateActiveCartAsync(int userId)
        {
            // Försök hämta en befintlig aktiv varukorg med alla items och filmdata
            var cart = await _db.Carts
                .Include(c => c.Items)              // Inkludera varukorgens items
                    .ThenInclude(i => i.Movie)      // Inkludera varje items tillhörande film
                .FirstOrDefaultAsync(c => c.UserId == userId && c.Status == CartStatus.Active);     //Filtrera på userId och att status är Active

            // Om ingen aktiv varukorg finns, skapa en ny
            if (cart == null)
            {
                cart = new Models.Cart
                {
                    UserId = userId,
                    Status = CartStatus.Active
                };

                _db.Carts.Add(cart);
                await _db.SaveChangesAsync();
            }

            return cart;
        }


        /// <summary>
        /// Omvandlar en Cart entitet från databasen till en CartDto.
        /// Vi gör detta i en egen metod eftersom flera publika metoder
        /// behöver returnera samma typ av data till klienten.
        /// </summary>
        private static CartDto MapToCartDto(Models.Cart cart)
        {
            return new CartDto
            {
                Id = cart.Id,

                Items = cart.Items.Select(i => new CartItemDto
                {
                    Id = i.Id,
                    MovieId = i.MovieId,
                    Title = i.Movie?.Title ?? "Okänd film",
                    PosterUrl = i.Movie?.PosterUrl,
                    UnitPrice = i.UnitPrice,
                    Quantity = i.Quantity,
                    AddedAt = i.AddedAt
                }).ToList(),
                TotalItems = cart.Items.Count,
                TotalPrice = cart.Items.Sum(i => i.UnitPrice * i.Quantity)
            };
        }
    }
}
