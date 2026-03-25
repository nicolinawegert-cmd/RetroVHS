using Microsoft.AspNetCore.Components.Authorization;
using RetroVHS.Client.Components;
using RetroVHS.Client.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5275")
});

// Auth: JwtAuthStateProvider registreras som både AuthenticationStateProvider och IAppAuthStateProvider
builder.Services.AddScoped<JwtAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<JwtAuthStateProvider>());
builder.Services.AddScoped<IAppAuthStateProvider>(sp => sp.GetRequiredService<JwtAuthStateProvider>());
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();

builder.Services.AddScoped<IAuthClient, AuthClient>();
builder.Services.AddScoped<IMovieClient, MovieClient>();
builder.Services.AddScoped<IReviewClient, ReviewClient>();
builder.Services.AddScoped<ICartClient, CartClient>();
builder.Services.AddScoped<CartState>();
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
