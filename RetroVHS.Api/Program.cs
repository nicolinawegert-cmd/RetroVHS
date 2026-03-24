using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RetroVHS.Api.Data;
using RetroVHS.Api.Models;
using RetroVHS.Api.Services.Auth;
using RetroVHS.Api.Services.Cart;
using RetroVHS.Api.Services.Movies;
using RetroVHS.Api.Services.Rentals;
using RetroVHS.Api.Services.Reviews;
using RetroVHS.Api.Services.Users;
using RetroVHS.Api.Services.Wishlists;

var builder = WebApplication.CreateBuilder(args);

// =========================
// Databas
// =========================
// Vi använder SQLite för att få en lokal databasfil.
// Connection string hämtas från appsettings.json.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// =========================
// Identity
// =========================
// Vi använder ASP.NET Identity som grund för användare och roller.
// ApplicationUser är vår egen användarmodell som ärver från IdentityUser<int>.
builder.Services
    .AddIdentityCore<ApplicationUser>(options =>
    {
        // Lösenordsregler som vi kan justera senare vid behov
        options.Password.RequiredLength = 6;
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireNonAlphanumeric = false;

        // E-post kan krävas som unik inloggningsuppgift
        options.User.RequireUniqueEmail = true;
    })
    .AddRoles<IdentityRole<int>>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();


builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IMovieService, MovieService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IRentalService, RentalService>();
builder.Services.AddScoped<IWishlistService, WishlistService>();


// =========================
// JWT Authentication
// =========================
// Här konfigurerar vi hur inkommande JWT-tokens ska valideras.
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Jwt:Key saknas i appsettings.json");

var jwtIssuer = builder.Configuration["Jwt:Issuer"]
    ?? throw new InvalidOperationException("Jwt:Issuer saknas i appsettings.json");

var jwtAudience = builder.Configuration["Jwt:Audience"]
    ?? throw new InvalidOperationException("Jwt:Audience saknas i appsettings.json");

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // Validera att token är signerad med rätt nyckel
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),

            // Validera utfärdare
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,

            // Validera mottagare
            ValidateAudience = true,
            ValidAudience = jwtAudience,

            // Validera tokenens livslängd
            ValidateLifetime = true,

            // Ingen extra tolerans på utgångstid
            ClockSkew = TimeSpan.Zero
        };
    });

// =========================
// Authorization
// =========================
builder.Services.AddAuthorization();

// =========================
// CORS
// =========================
// Bra när Blazor-klienten körs separat från API:t under utveckling.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClient", policy =>
    {
        policy
            .WithOrigins(
                "https://localhost:7220",
                "http://localhost:5084"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// =========================
// Controllers + Swagger
// =========================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();


builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "RetroVHS API",
        Version = "v1"
    });

    // 🔐 Lägg till JWT auth i Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Skriv: Bearer {din token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});


var app = builder.Build();

// =========================
// Automatisk migrering och seedning vid appstart
// =========================
// Vi ser till att databasen alltid uppdateras till senaste migration
// när applikationen startar. Därefter kan vi köra seedning av grunddata.
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var dbContext = services.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.MigrateAsync();
    await DbSeeder.SeedAsync(services);

}

// =========================
// Middleware pipeline
// =========================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowClient");

// Authentication måste komma före Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();