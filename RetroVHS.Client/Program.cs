using Microsoft.AspNetCore.Components.Authorization;
using RetroVHS.Client.Components;
using RetroVHS.Client.Services;

var builder = WebApplication.CreateBuilder(args);

// Blazor Server med Interactive Server Components (SignalR-baserat, all C# körs på servern)
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// En HttpClient per SignalR-krets (Scoped), med API-adressen från appsettings.json
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5275")
});

// Auth-setup:
// JwtAuthStateProvider registreras EN GÅNG som konkret typ,
// men aliasas till BÅDA interfaces så att samma instans injiceras oavsett vilket interface som begärs.
// Det gör att komponenter kan använda IAppAuthStateProvider (för mutation) utan att känna till JwtAuthStateProvider.
builder.Services.AddScoped<JwtAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<JwtAuthStateProvider>());
builder.Services.AddScoped<IAppAuthStateProvider>(sp => sp.GetRequiredService<JwtAuthStateProvider>());
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState(); // Gör auth-state tillgänglig i hela komponentträdet

// HTTP-klienter registrerade mot sina interface — möjliggör testning och löskoppling
builder.Services.AddScoped<IAuthClient, AuthClient>();
builder.Services.AddScoped<IMovieClient, MovieClient>();
builder.Services.AddScoped<IReviewClient, ReviewClient>(); // Separerat från IMovieClient (ISP)
builder.Services.AddScoped<ICartClient, CartClient>();
builder.Services.AddScoped<CartState>();                   // Delad state för varukorgsräknaren i headern
builder.Services.AddScoped<IWishlistClient, WishlistClient>();
builder.Services.AddScoped<IUserClient, UserClient>();
builder.Services.AddScoped<IAdminClient, AdminClient>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
