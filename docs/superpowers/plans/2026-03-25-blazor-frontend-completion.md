# RetroVHS Blazor Frontend Completion — Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Complete the RetroVHS Blazor frontend: user rental actions, a shared movie list, three API reference endpoints, and a full admin UI section with dashboard, user management, and movie management.

**Architecture:** All new Blazor pages follow the existing SSR (Interactive Server) pattern — `@rendermode InteractiveServer` where needed, services injected via `@inject`, loading state with spinners, inline confirmations (no modals). The admin service (`IAdminClient`/`AdminClient`) mirrors the pattern of existing clients (`UserClient`, `MovieClient`). Three lightweight read-only API controllers are added to support the admin movie form's genre/person/company lookups.

**Tech Stack:** Blazor Server (.NET 9), ASP.NET Core Web API, Entity Framework Core, Bootstrap 5 (via existing CDN), existing CSS design system (`#e94560` accent, dark theme). No test projects — tests are explicitly out of scope for this project.

**IMPORTANT — Design Conventions:**
- Invoke the `frontend-design` skill before creating or modifying any `.razor` file
- Reuse existing CSS classes from `app.css`: `section-title`, `btn-submit`, `btn-remove`, `cart-item`, `cart-item-info`, `loading-spinner`, `review-error`, `profile-success`, `user-menu-item`, `user-menu-divider`, etc.
- **Inline confirmation pattern:** clicking a destructive button shows a `<div class="confirm-inline">` directly below it with text and "Ja" / "Avbryt" buttons. Never use `window.confirm()` or modals.
- Status badges: `<span class="rental-status status-active">` etc. New CSS classes `rental-status`, `status-active`, `status-completed`, `status-cancelled`, `status-expired` are added to `app.css` in Task 1.

---

## File Map

### RetroVHS.Shared (new DTOs)
- `DTOs/Genres/GenreDto.cs` — `int Id`, `string Name`
- `DTOs/Persons/PersonDto.cs` — `int Id`, `string FullName`
- `DTOs/ProductionCompanies/ProductionCompanyDto.cs` — `int Id`, `string Name`

### RetroVHS.Api (new controllers)
- `Controllers/GenresController.cs` — `GET /api/genres`
- `Controllers/PersonsController.cs` — `GET /api/persons?search={term}`
- `Controllers/ProductionCompaniesController.cs` — `GET /api/production-companies`

### RetroVHS.Client — Services
- `Services/IAdminClient.cs` — full admin interface (20 methods)
- `Services/AdminClient.cs` — HTTP implementation
- Modify `Services/IUserClient.cs` — add `CompleteRentalAsync`, `CancelRentalAsync`
- Modify `Services/UserClient.cs` — implement the two new methods
- Modify `Program.cs` — register `IAdminClient`

### RetroVHS.Client — Pages (new)
- `Components/Pages/Movies.razor` — shared movie list with genre filter + sort
- `Components/Pages/Admin/Dashboard.razor` — stat cards
- `Components/Pages/Admin/Users.razor` — user table
- `Components/Pages/Admin/UserProfile.razor` — single user admin view
- `Components/Pages/Admin/UserReviews.razor` — user's reviews admin view
- `Components/Pages/Admin/UserOrders.razor` — user's orders admin view
- `Components/Pages/Admin/MovieList.razor` — admin movie list
- `Components/Pages/Admin/MovieForm.razor` — create/edit movie form

### RetroVHS.Client — Pages (modified)
- `Components/Pages/Home.razor` — add "Se alla filmer →" link, remove TODO comment
- `Components/Pages/MyOrders.razor` — status badges + inline confirm/cancel actions
- `Components/Pages/MovieDetails.razor` — "Anonym" fallback
- `Components/Layout/Header.razor` — admin link for Admin role

### RetroVHS.Client — Styles
- `wwwroot/app.css` — add rental status badge classes + `confirm-inline` styles

---

## Task 1: CSS Additions

**Files:**
- Modify: `RetroVHS.Client/wwwroot/app.css`

- [ ] **Step 1: Add status badge and inline confirmation CSS**

Append to `app.css`:

```css
/* ── Rental status badges ─────────────────────────────── */
.rental-status {
    display: inline-block;
    padding: 2px 10px;
    border-radius: 12px;
    font-size: 0.78rem;
    font-weight: 600;
    letter-spacing: 0.02em;
}
.status-active    { background: rgba(233,69,96,0.15); color: #e94560; border: 1px solid #e94560; }
.status-completed { background: rgba(80,200,120,0.12); color: #50c878; border: 1px solid #50c878; }
.status-cancelled { background: rgba(160,160,176,0.12); color: #a0a0b0; border: 1px solid #a0a0b0; }
.status-expired   { background: rgba(160,160,176,0.12); color: #a0a0b0; border: 1px solid #a0a0b0; }

/* ── Inline confirmation ──────────────────────────────── */
.confirm-inline {
    margin-top: 0.5rem;
    padding: 0.6rem 0.8rem;
    background: rgba(233,69,96,0.08);
    border: 1px solid rgba(233,69,96,0.3);
    border-radius: 6px;
    display: flex;
    align-items: center;
    gap: 0.75rem;
    flex-wrap: wrap;
}
.confirm-inline p {
    margin: 0;
    color: #c0c0d0;
    font-size: 0.9rem;
    flex: 1 1 100%;
}
.confirm-inline .btn-confirm-yes {
    background: #e94560;
    color: #fff;
    border: none;
    padding: 5px 14px;
    border-radius: 4px;
    font-size: 0.85rem;
    cursor: pointer;
}
.confirm-inline .btn-confirm-yes:hover { background: #c73652; }
.confirm-inline .btn-confirm-cancel {
    background: transparent;
    color: #a0a0b0;
    border: 1px solid #a0a0b0;
    padding: 5px 14px;
    border-radius: 4px;
    font-size: 0.85rem;
    cursor: pointer;
}
.confirm-inline .btn-confirm-cancel:hover { color: #e0e0f0; border-color: #e0e0f0; }

/* ── Admin pages ──────────────────────────────────────── */
.admin-page { padding: 2rem 1.5rem; max-width: 1100px; margin: 0 auto; }
.admin-stat-grid {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(170px, 1fr));
    gap: 1rem;
    margin: 1.5rem 0 2rem;
}
.admin-stat-card {
    background: #16213e;
    border: 1px solid rgba(255,255,255,0.07);
    border-radius: 8px;
    padding: 1.2rem 1rem;
    text-align: center;
}
.admin-stat-value { font-size: 2rem; font-weight: 700; color: #e94560; }
.admin-stat-label { font-size: 0.8rem; color: #a0a0b0; margin-top: 0.25rem; }
.admin-nav-links { display: flex; gap: 1rem; flex-wrap: wrap; margin-bottom: 2rem; }
.admin-nav-links a {
    background: #16213e;
    border: 1px solid rgba(255,255,255,0.1);
    color: #e0e0f0;
    padding: 0.5rem 1.2rem;
    border-radius: 6px;
    text-decoration: none;
    font-size: 0.9rem;
    transition: border-color 0.2s;
}
.admin-nav-links a:hover { border-color: #e94560; color: #e94560; }

.admin-table { width: 100%; border-collapse: collapse; font-size: 0.9rem; }
.admin-table th {
    text-align: left;
    padding: 0.6rem 0.8rem;
    border-bottom: 1px solid rgba(255,255,255,0.1);
    color: #a0a0b0;
    font-weight: 600;
}
.admin-table td {
    padding: 0.6rem 0.8rem;
    border-bottom: 1px solid rgba(255,255,255,0.05);
    vertical-align: middle;
}
.admin-table tr:hover td { background: rgba(255,255,255,0.02); }
.admin-table .action-links { display: flex; gap: 0.5rem; align-items: center; flex-wrap: wrap; }
.admin-table .action-links a { color: #a0c4ff; font-size: 0.82rem; text-decoration: none; }
.admin-table .action-links a:hover { color: #e94560; }

.admin-back-link { display: inline-block; color: #a0a0b0; font-size: 0.85rem; margin-bottom: 1.2rem; text-decoration: none; }
.admin-back-link:hover { color: #e94560; }
.admin-user-info { background: #16213e; border-radius: 8px; padding: 1.2rem 1.5rem; margin-bottom: 1.5rem; }
.admin-user-info p { margin: 0.3rem 0; color: #c0c0d0; font-size: 0.9rem; }
.admin-user-info strong { color: #e0e0f0; }
.admin-actions { display: flex; flex-direction: column; gap: 0.5rem; max-width: 400px; }

/* ── Movie list page ──────────────────────────────────── */
.movies-page { padding: 2rem 1.5rem; max-width: 900px; margin: 0 auto; }
.movies-controls {
    display: flex;
    gap: 1rem;
    flex-wrap: wrap;
    align-items: center;
    margin-bottom: 1.5rem;
}
.movies-controls select {
    background: #16213e;
    color: #e0e0f0;
    border: 1px solid rgba(255,255,255,0.15);
    border-radius: 6px;
    padding: 0.4rem 0.8rem;
    font-size: 0.9rem;
}
.movies-list { display: flex; flex-direction: column; gap: 0; }
.movie-list-row {
    display: flex;
    align-items: center;
    gap: 1rem;
    padding: 0.75rem 0;
    border-bottom: 1px solid rgba(255,255,255,0.06);
    text-decoration: none;
    color: inherit;
}
.movie-list-row:hover .movie-list-title { color: #e94560; }
.movie-list-title { font-size: 1rem; font-weight: 600; color: #e0e0f0; flex: 1; }
.movie-list-year { color: #a0a0b0; font-size: 0.85rem; min-width: 3.5rem; }
.movie-list-rating { color: #f5c518; font-size: 0.85rem; min-width: 6rem; }
.movie-list-rating.no-rating { color: #606070; }
.movie-list-actions { display: flex; gap: 0.5rem; margin-left: auto; }
.movie-list-actions a { font-size: 0.82rem; color: #a0c4ff; text-decoration: none; }
.movie-list-actions a:hover { color: #e94560; }
.see-all-link {
    display: inline-block;
    color: #e94560;
    font-size: 0.9rem;
    text-decoration: none;
    margin-bottom: 1rem;
    font-weight: 500;
}
.see-all-link:hover { text-decoration: underline; }

/* ── Movie form (admin) ───────────────────────────────── */
.movie-form-page { padding: 2rem 1.5rem; max-width: 800px; margin: 0 auto; }
.movie-form-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 1rem; }
.movie-form-grid .full-width { grid-column: 1 / -1; }
.movie-form-field { display: flex; flex-direction: column; gap: 0.3rem; }
.movie-form-field label { font-size: 0.85rem; color: #a0a0b0; font-weight: 500; }
.movie-form-field input,
.movie-form-field select,
.movie-form-field textarea {
    background: #16213e;
    color: #e0e0f0;
    border: 1px solid rgba(255,255,255,0.15);
    border-radius: 6px;
    padding: 0.45rem 0.75rem;
    font-size: 0.9rem;
}
.movie-form-field input:focus,
.movie-form-field select:focus,
.movie-form-field textarea:focus {
    outline: none;
    border-color: #e94560;
}
.movie-form-field .validation-message { color: #e94560; font-size: 0.8rem; }

.genre-tag-list { display: flex; flex-wrap: wrap; gap: 0.4rem; margin-top: 0.3rem; }
.genre-tag {
    background: rgba(233,69,96,0.15);
    border: 1px solid rgba(233,69,96,0.4);
    color: #e94560;
    border-radius: 12px;
    padding: 2px 10px;
    font-size: 0.8rem;
    cursor: pointer;
    transition: background 0.15s;
}
.genre-tag.selected { background: #e94560; color: #fff; }
.genre-tag:hover { background: rgba(233,69,96,0.3); }
.genre-tag.selected:hover { background: #c73652; }

.person-search-row { display: flex; flex-direction: column; gap: 0.3rem; position: relative; margin-bottom: 0.5rem; }
.person-search-row .person-row-inputs { display: flex; gap: 0.5rem; align-items: flex-start; }
.person-search-result-dropdown {
    position: absolute;
    top: calc(100% + 2px);
    left: 0;
    right: 0;
    background: #16213e;
    border: 1px solid rgba(255,255,255,0.15);
    border-radius: 6px;
    z-index: 100;
    max-height: 160px;
    overflow-y: auto;
}
.person-result-item {
    padding: 0.4rem 0.75rem;
    cursor: pointer;
    font-size: 0.88rem;
    color: #e0e0f0;
}
.person-result-item:hover { background: rgba(233,69,96,0.1); color: #e94560; }
.btn-add-row {
    background: transparent;
    border: 1px dashed rgba(255,255,255,0.2);
    color: #a0a0b0;
    border-radius: 6px;
    padding: 0.35rem 1rem;
    font-size: 0.85rem;
    cursor: pointer;
    transition: border-color 0.15s, color 0.15s;
    margin-top: 0.25rem;
}
.btn-add-row:hover { border-color: #e94560; color: #e94560; }
.btn-remove-row {
    background: transparent;
    border: none;
    color: #606070;
    cursor: pointer;
    font-size: 1rem;
    padding: 0 0.4rem;
    line-height: 1;
}
.btn-remove-row:hover { color: #e94560; }
```

- [ ] **Step 2: Commit**

```bash
git add RetroVHS.Client/wwwroot/app.css
git commit -m "style: add rental status badges, inline confirm, admin and movie-list CSS"
```

---

## Task 2: Extend MovieDetailsDto for Admin Edit Pre-fill

**Files:**
- Modify: `RetroVHS.Shared/DTOs/Movies/MovieDetailsDto.cs`
- Modify: `RetroVHS.Api/Services/Movies/MovieService.cs` (the `GetMovieByIdAsync` projection)

`MovieDetailsDto` currently lacks `Language`, `Country`, `ProductionCompanyId`, and `IsFeatured`. These are needed to pre-fill the admin movie edit form.

- [ ] **Step 1: Add missing fields to MovieDetailsDto**

In `RetroVHS.Shared/DTOs/Movies/MovieDetailsDto.cs`, add after `StockQuantity`:

```csharp
/// <summary>Anger om filmen ska lyftas extra</summary>
public bool IsFeatured { get; set; }

/// <summary>Språk filmen är på</summary>
public string? Language { get; set; }

/// <summary>Ursprungsland</summary>
public string? Country { get; set; }

/// <summary>Id för produktionsbolag (om kopplat)</summary>
public int? ProductionCompanyId { get; set; }
```

- [ ] **Step 2: Update MovieService projection**

In `RetroVHS.Api/Services/Movies/MovieService.cs`, in `GetMovieByIdAsync`, the return statement builds a `MovieDetailsDto`. Add the four new fields to the object initializer:

```csharp
IsFeatured = movie.IsFeatured,
Language = movie.Language,
Country = movie.Country,
ProductionCompanyId = movie.ProductionCompanyId,
```

- [ ] **Step 3: Commit**

```bash
git add RetroVHS.Shared/DTOs/Movies/MovieDetailsDto.cs RetroVHS.Api/Services/Movies/MovieService.cs
git commit -m "feat: add Language, Country, IsFeatured, ProductionCompanyId to MovieDetailsDto"
```

---

## Task 3: Shared DTOs for Reference Data

**Files:**
- Create: `RetroVHS.Shared/DTOs/Genres/GenreDto.cs`
- Create: `RetroVHS.Shared/DTOs/Persons/PersonDto.cs`
- Create: `RetroVHS.Shared/DTOs/ProductionCompanies/ProductionCompanyDto.cs`

- [ ] **Step 1: Create GenreDto**

`RetroVHS.Shared/DTOs/Genres/GenreDto.cs`:
```csharp
namespace RetroVHS.Shared.DTOs.Genres;

public class GenreDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
```

- [ ] **Step 2: Create PersonDto**

`RetroVHS.Shared/DTOs/Persons/PersonDto.cs`:
```csharp
namespace RetroVHS.Shared.DTOs.Persons;

public class PersonDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
}
```

- [ ] **Step 3: Create ProductionCompanyDto**

`RetroVHS.Shared/DTOs/ProductionCompanies/ProductionCompanyDto.cs`:
```csharp
namespace RetroVHS.Shared.DTOs.ProductionCompanies;

public class ProductionCompanyDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
```

- [ ] **Step 4: Commit**

```bash
git add RetroVHS.Shared/DTOs/
git commit -m "feat: add GenreDto, PersonDto, ProductionCompanyDto to Shared"
```

---

## Task 3: API Reference Endpoints

**Files:**
- Create: `RetroVHS.Api/Controllers/GenresController.cs`
- Create: `RetroVHS.Api/Controllers/PersonsController.cs`
- Create: `RetroVHS.Api/Controllers/ProductionCompaniesController.cs`

The `ApplicationDbContext` already has `DbSet<Genre>`, `DbSet<Person>`, and `DbSet<ProductionCompany>` — inject it directly in each controller (no new service layer needed for simple read-only queries).

- [ ] **Step 1: Create GenresController**

`RetroVHS.Api/Controllers/GenresController.cs`:
```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RetroVHS.Api.Data;
using RetroVHS.Shared.DTOs.Genres;

namespace RetroVHS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GenresController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public GenresController(ApplicationDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<List<GenreDto>>> GetAll()
    {
        var genres = await _context.Genres
            .OrderBy(g => g.Name)
            .Select(g => new GenreDto { Id = g.Id, Name = g.Name })
            .ToListAsync();
        return Ok(genres);
    }
}
```

- [ ] **Step 2: Create PersonsController**

`RetroVHS.Api/Controllers/PersonsController.cs`:
```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RetroVHS.Api.Data;
using RetroVHS.Shared.DTOs.Persons;

namespace RetroVHS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PersonsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public PersonsController(ApplicationDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<List<PersonDto>>> GetAll([FromQuery] string? search)
    {
        var query = _context.Persons.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(p => EF.Functions.Like(p.FullName, $"%{search}%"));

        var persons = await query
            .OrderBy(p => p.FullName)
            .Select(p => new PersonDto { Id = p.Id, FullName = p.FullName })
            .Take(30)
            .ToListAsync();
        return Ok(persons);
    }
}
```

- [ ] **Step 3: Create ProductionCompaniesController**

`RetroVHS.Api/Controllers/ProductionCompaniesController.cs`:
```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RetroVHS.Api.Data;
using RetroVHS.Shared.DTOs.ProductionCompanies;

namespace RetroVHS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductionCompaniesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public ProductionCompaniesController(ApplicationDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<List<ProductionCompanyDto>>> GetAll()
    {
        var companies = await _context.ProductionCompanies
            .OrderBy(c => c.Name)
            .Select(c => new ProductionCompanyDto { Id = c.Id, Name = c.Name })
            .ToListAsync();
        return Ok(companies);
    }
}
```

- [ ] **Step 4: Verify DbContext has the required DbSets**

Check `RetroVHS.Api/Data/ApplicationDbContext.cs` to confirm `DbSet<Genre> Genres`, `DbSet<Person> Persons`, and `DbSet<ProductionCompany> ProductionCompanies` exist. If any are missing, add them.

- [ ] **Step 5: Commit**

```bash
git add RetroVHS.Api/Controllers/GenresController.cs RetroVHS.Api/Controllers/PersonsController.cs RetroVHS.Api/Controllers/ProductionCompaniesController.cs
git commit -m "feat(api): add read-only genres, persons, production-companies endpoints"
```

---

## Task 4: IUserClient Extensions (Complete/Cancel Rental)

**Files:**
- Modify: `RetroVHS.Client/Services/IUserClient.cs`
- Modify: `RetroVHS.Client/Services/UserClient.cs`

- [ ] **Step 1: Add methods to IUserClient**

In `IUserClient.cs`, add to the interface:
```csharp
Task<bool> CompleteRentalAsync(int rentalId);  // PUT /api/rentals/{id}/complete
Task<bool> CancelRentalAsync(int rentalId);    // PUT /api/rentals/{id}/cancel
```

- [ ] **Step 2: Implement in UserClient**

In `UserClient.cs`, add:
```csharp
public async Task<bool> CompleteRentalAsync(int rentalId)
{
    try
    {
        var response = await _httpClient.PutAsync($"api/rentals/{rentalId}/complete", null);
        return response.IsSuccessStatusCode;
    }
    catch { return false; }
}

public async Task<bool> CancelRentalAsync(int rentalId)
{
    try
    {
        var response = await _httpClient.PutAsync($"api/rentals/{rentalId}/cancel", null);
        return response.IsSuccessStatusCode;
    }
    catch { return false; }
}
```

- [ ] **Step 3: Commit**

```bash
git add RetroVHS.Client/Services/IUserClient.cs RetroVHS.Client/Services/UserClient.cs
git commit -m "feat: add CompleteRentalAsync and CancelRentalAsync to IUserClient"
```

---

## Task 5: Quick UI Fixes

**Files:**
- Modify: `RetroVHS.Client/Components/Pages/MovieDetails.razor`
- Modify: `RetroVHS.Client/Components/Pages/Home.razor`

- [ ] **Step 1: Fix "Anonym" fallback in MovieDetails**

In `MovieDetails.razor`, in the review list loop, find:
```razor
<span class="review-user">@review.UserDisplayName</span>
```
Change to:
```razor
<span class="review-user">@(string.IsNullOrWhiteSpace(review.UserDisplayName) ? "Anonym" : review.UserDisplayName)</span>
```

- [ ] **Step 2: Add "Se alla filmer" link and remove TODO in Home**

In `Home.razor`:
1. After `<PageTitle>RetroVHS</PageTitle>` and before the `@if (isLoading)` block, add:
```razor
<div style="padding: 1rem 1.5rem 0;">
    <a href="/movies" class="see-all-link">Se alla filmer →</a>
</div>
```

2. Remove lines 39–55 (the TODO comment and the commented-out allMovies block).

3. Remove the `private List<MovieListDto> allMovies = [];` field and `allMovies = await allTask;` line since `allMovies` is no longer used. Remove `var allTask = MovieClient.GetAllMoviesAsync();` as well.

- [ ] **Step 3: Commit**

```bash
git add RetroVHS.Client/Components/Pages/MovieDetails.razor RetroVHS.Client/Components/Pages/Home.razor
git commit -m "fix: show Anonym for null review display name; add See All Movies link to Home"
```

---

## Task 6: MyOrders — Status Badges + Rental Actions

**Files:**
- Modify: `RetroVHS.Client/Components/Pages/MyOrders.razor`

Replace the entire file. The new version:
- Shows a status badge per rental using the CSS classes from Task 1
- For `Active` rentals: shows "Bekräfta leverans" and "Avbryt beställning" buttons
- Clicking either button shows an inline `confirm-inline` div
- On confirmation: calls `CompleteRentalAsync` or `CancelRentalAsync`, updates status in-place

- [ ] **Step 1: Rewrite MyOrders.razor**

```razor
@page "/my-orders"
@using RetroVHS.Shared.DTOs.Rentals
@using RetroVHS.Shared.Enums
@inject IUserClient UserClient
@inject NavigationManager Navigation

<PageTitle>Mina beställningar</PageTitle>

<AuthorizeView>
    <Authorized>
        <div class="cart-page">
            <h1 class="section-title">📦 Mina beställningar</h1>

            @if (isLoading)
            {
                <div class="loading-spinner">
                    <div class="spinner-border" role="status" style="color: #e94560; width: 2.5rem; height: 2.5rem;"></div>
                    <p>Laddar beställningar...</p>
                </div>
            }
            else if (orders.Count == 0)
            {
                <p style="color: #a0a0b0;">Du har inga beställningar ännu.</p>
            }
            else
            {
                <div class="cart-items">
                    @foreach (var order in orders)
                    {
                        <div class="cart-item">
                            <div class="cart-item-info">
                                <h3 style="cursor:pointer" @onclick="() => Navigation.NavigateTo($\"/movie/{order.MovieId}\")">@order.Title</h3>
                                <p class="cart-item-price">@order.PricePaid.ToString("0.00") kr</p>
                                <p style="color: #a0a0b0; font-size: 0.85rem; margin-top: 4px;">
                                    Köpt @order.RentedAt.ToString("d MMM yyyy")
                                </p>
                                <div style="margin-top:6px;">
                                    <span class="rental-status @GetStatusClass(order.Status)">@GetStatusLabel(order.Status)</span>
                                </div>

                                @if (order.Status == RentalStatus.Active)
                                {
                                    @if (confirmingId == order.Id && confirmAction == "complete")
                                    {
                                        <div class="confirm-inline">
                                            <p>Bekräfta att du har mottagit filmen?</p>
                                            <button class="btn-confirm-yes" @onclick="() => ExecuteActionAsync(order, \"complete\")" disabled="@acting">
                                                @(acting ? "..." : "Ja, bekräfta")
                                            </button>
                                            <button class="btn-confirm-cancel" @onclick="CancelConfirm" disabled="@acting">Avbryt</button>
                                        </div>
                                    }
                                    else if (confirmingId == order.Id && confirmAction == "cancel")
                                    {
                                        <div class="confirm-inline">
                                            <p>Vill du avbryta beställningen?</p>
                                            <button class="btn-confirm-yes" @onclick="() => ExecuteActionAsync(order, \"cancel\")" disabled="@acting">
                                                @(acting ? "..." : "Ja, avbryt")
                                            </button>
                                            <button class="btn-confirm-cancel" @onclick="CancelConfirm" disabled="@acting">Nej</button>
                                        </div>
                                    }
                                    else
                                    {
                                        <div style="display:flex; gap:0.5rem; margin-top:0.5rem; flex-wrap:wrap;">
                                            <button class="btn-submit" style="font-size:0.82rem;padding:5px 12px;"
                                                    @onclick="() => ShowConfirm(order.Id, \"complete\")">
                                                ✅ Bekräfta leverans
                                            </button>
                                            <button class="btn-remove" style="font-size:0.82rem;padding:5px 12px;"
                                                    @onclick="() => ShowConfirm(order.Id, \"cancel\")">
                                                Avbryt beställning
                                            </button>
                                        </div>
                                    }
                                }
                                @if (!string.IsNullOrEmpty(actionError) && actionErrorId == order.Id)
                                {
                                    <p class="review-error" style="margin-top:0.4rem;">@actionError</p>
                                }
                            </div>
                        </div>
                    }
                </div>
            }
        </div>
    </Authorized>
    <NotAuthorized>
        <div class="cart-page">
            <p>Du måste logga in för att se dina beställningar.</p>
        </div>
    </NotAuthorized>
</AuthorizeView>

@code {
    private List<RentalDto> orders = [];
    private bool isLoading = true;

    private int? confirmingId;
    private string? confirmAction;
    private bool acting;
    private string? actionError;
    private int? actionErrorId;

    protected override async Task OnInitializedAsync()
    {
        orders = await UserClient.GetMyOrdersAsync();
        isLoading = false;
    }

    private void ShowConfirm(int id, string action)
    {
        confirmingId = id;
        confirmAction = action;
        actionError = null;
        actionErrorId = null;
    }

    private void CancelConfirm()
    {
        confirmingId = null;
        confirmAction = null;
    }

    private async Task ExecuteActionAsync(RentalDto order, string action)
    {
        acting = true;
        actionError = null;
        bool success = action == "complete"
            ? await UserClient.CompleteRentalAsync(order.Id)
            : await UserClient.CancelRentalAsync(order.Id);

        if (success)
        {
            order.Status = action == "complete" ? RentalStatus.Completed : RentalStatus.Cancelled;
            confirmingId = null;
            confirmAction = null;
        }
        else
        {
            actionError = "Åtgärden misslyckades. Försök igen.";
            actionErrorId = order.Id;
        }
        acting = false;
    }

    private static string GetStatusClass(RentalStatus s) => s switch
    {
        RentalStatus.Active    => "status-active",
        RentalStatus.Completed => "status-completed",
        RentalStatus.Cancelled => "status-cancelled",
        RentalStatus.Expired   => "status-expired",
        _ => ""
    };

    private static string GetStatusLabel(RentalStatus s) => s switch
    {
        RentalStatus.Active    => "Aktiv",
        RentalStatus.Completed => "Levererad",
        RentalStatus.Cancelled => "Avbruten",
        RentalStatus.Expired   => "Utgången",
        _ => s.ToString()
    };
}
```

- [ ] **Step 2: Commit**

```bash
git add RetroVHS.Client/Components/Pages/MyOrders.razor
git commit -m "feat: add rental status badges and confirm/cancel actions to MyOrders"
```

---

## Task 7: Shared Movie List Page `/movies`

**Files:**
- Create: `RetroVHS.Client/Components/Pages/Movies.razor`

- [ ] **Step 1: Create Movies.razor**

```razor
@page "/movies"
@using RetroVHS.Shared.DTOs.Movies
@using Microsoft.AspNetCore.Components.Authorization
@inject IMovieClient MovieClient
@inject NavigationManager Navigation
@inject AuthenticationStateProvider AuthStateProvider

<PageTitle>Alla filmer</PageTitle>

<div class="movies-page">
    <h1 class="section-title">🎬 Alla filmer</h1>

    @if (isLoading)
    {
        <div class="loading-spinner">
            <div class="spinner-border" role="status" style="color:#e94560;width:2.5rem;height:2.5rem;"></div>
            <p>Laddar filmer...</p>
        </div>
    }
    else
    {
        <div class="movies-controls">
            <select @bind="selectedGenre" @bind:after="ApplyFilter">
                <option value="">Alla genres</option>
                @foreach (var g in allGenres)
                {
                    <option value="@g">@g</option>
                }
            </select>

            <select @bind="sortOption" @bind:after="ApplyFilter">
                <option value="title-asc">Titel A→Z</option>
                <option value="title-desc">Titel Z→A</option>
                <option value="rating-desc">Betyg (högt→lågt)</option>
            </select>
        </div>

        @if (filtered.Count == 0)
        {
            <p style="color:#a0a0b0;">Inga filmer matchar filtret.</p>
        }
        else
        {
            <div class="movies-list">
                @foreach (var movie in filtered)
                {
                    <div class="movie-list-row">
                        <span class="movie-list-year">@movie.ReleaseYear</span>
                        <a class="movie-list-title" href="/movie/@movie.Id">@movie.Title</a>
                        @if (movie.RatingCount > 0)
                        {
                            <span class="movie-list-rating">★ @movie.RatingAverage.ToString("0.0") <span style="color:#606070;font-size:0.8rem;">(@movie.RatingCount)</span></span>
                        }
                        else
                        {
                            <span class="movie-list-rating no-rating">Inga betyg</span>
                        }
                        <div class="movie-list-actions">
                            @if (isAdmin)
                            {
                                <a href="/admin/movies/@movie.Id/edit">Redigera</a>
                            }
                        </div>
                    </div>
                }
            </div>
        }
    }
</div>

@code {
    private List<MovieListDto> allMovies = [];
    private List<MovieListDto> filtered = [];
    private List<string> allGenres = [];
    private string selectedGenre = "";
    private string sortOption = "title-asc";
    private bool isLoading = true;
    private bool isAdmin;

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        isAdmin = authState.User.IsInRole("Admin");
        allMovies = await MovieClient.GetAllMoviesAsync();
        allGenres = allMovies
            .SelectMany(m => m.Genres)
            .Distinct()
            .OrderBy(g => g)
            .ToList();
        isLoading = false;
        ApplyFilter();
    }

    private void ApplyFilter()
    {
        var result = allMovies.AsEnumerable();
        if (!string.IsNullOrEmpty(selectedGenre))
            result = result.Where(m => m.Genres.Contains(selectedGenre));

        result = sortOption switch
        {
            "title-desc"   => result.OrderByDescending(m => m.Title),
            "rating-desc"  => result.OrderByDescending(m => m.RatingAverage),
            _              => result.OrderBy(m => m.Title)
        };

        filtered = result.ToList();
    }
}
```

- [ ] **Step 2: Commit**

```bash
git add RetroVHS.Client/Components/Pages/Movies.razor
git commit -m "feat: add shared movie list page /movies with genre filter and sort"
```

---

## Task 8: IAdminClient + AdminClient + Program.cs Registration

**Files:**
- Create: `RetroVHS.Client/Services/IAdminClient.cs`
- Create: `RetroVHS.Client/Services/AdminClient.cs`
- Modify: `RetroVHS.Client/Program.cs`

- [ ] **Step 1: Create IAdminClient.cs**

`RetroVHS.Client/Services/IAdminClient.cs`:
```csharp
using RetroVHS.Shared.DTOs.Admin;
using RetroVHS.Shared.DTOs.Auth;
using RetroVHS.Shared.DTOs.Genres;
using RetroVHS.Shared.DTOs.Movies;
using RetroVHS.Shared.DTOs.Persons;
using RetroVHS.Shared.DTOs.ProductionCompanies;
using RetroVHS.Shared.DTOs.Rentals;
using RetroVHS.Shared.DTOs.Reviews;

namespace RetroVHS.Client.Services;

public interface IAdminClient
{
    // Dashboard
    Task<AdminDashboardDto?> GetDashboardStatsAsync();

    // Users
    Task<List<UserDto>> GetAllUsersAsync();
    Task<UserDto?> GetUserByIdAsync(int id);
    Task<bool> DeleteUserAsync(int id);
    Task<bool> BlockUserAsync(int id);
    Task<bool> UnblockUserAsync(int id);
    Task<bool> SetNicknameNullAsync(int id);

    // Reviews
    Task<List<ReviewDto>> GetUserReviewsAsync(int userId);
    Task<bool> RemoveReviewCommentAsync(int reviewId);
    Task<bool> DeleteReviewAsync(int reviewId);

    // Rentals
    Task<List<RentalDto>> GetUserRentalsAsync(int userId);
    Task<bool> CancelRentalAsync(int rentalId);

    // Movies
    Task<List<MovieListDto>> GetAllMoviesAsync();
    Task<MovieDetailsDto?> GetMovieByIdAsync(int id);
    Task<MovieDetailsDto?> CreateMovieAsync(CreateMovieDto dto);
    Task<MovieDetailsDto?> UpdateMovieAsync(int id, UpdateMovieDto dto);
    Task<bool> DeleteMovieAsync(int id);

    // Reference data
    Task<List<GenreDto>> GetGenresAsync();
    Task<List<PersonDto>> GetPersonsAsync(string? search = null);
    Task<List<ProductionCompanyDto>> GetProductionCompaniesAsync();
}
```

- [ ] **Step 2: Create AdminClient.cs**

`RetroVHS.Client/Services/AdminClient.cs`:
```csharp
using System.Net.Http.Json;
using RetroVHS.Shared.DTOs.Admin;
using RetroVHS.Shared.DTOs.Auth;
using RetroVHS.Shared.DTOs.Genres;
using RetroVHS.Shared.DTOs.Movies;
using RetroVHS.Shared.DTOs.Persons;
using RetroVHS.Shared.DTOs.ProductionCompanies;
using RetroVHS.Shared.DTOs.Rentals;
using RetroVHS.Shared.DTOs.Reviews;

namespace RetroVHS.Client.Services;

public class AdminClient : IAdminClient
{
    private readonly HttpClient _http;
    public AdminClient(HttpClient http) => _http = http;

    // ── Dashboard ──────────────────────────────────────────────────
    public async Task<AdminDashboardDto?> GetDashboardStatsAsync()
    {
        try { return await _http.GetFromJsonAsync<AdminDashboardDto>("api/admin/stats"); }
        catch { return null; }
    }

    // ── Users ──────────────────────────────────────────────────────
    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        try { return await _http.GetFromJsonAsync<List<UserDto>>("api/admin/users") ?? []; }
        catch { return []; }
    }

    public async Task<UserDto?> GetUserByIdAsync(int id)
    {
        try { return await _http.GetFromJsonAsync<UserDto>($"api/admin/users/{id}"); }
        catch { return null; }
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        try { return (await _http.DeleteAsync($"api/admin/users/{id}")).IsSuccessStatusCode; }
        catch { return false; }
    }

    public async Task<bool> BlockUserAsync(int id)
    {
        try { return (await _http.PutAsync($"api/admin/users/{id}/block", null)).IsSuccessStatusCode; }
        catch { return false; }
    }

    public async Task<bool> UnblockUserAsync(int id)
    {
        try { return (await _http.PutAsync($"api/admin/users/{id}/unblock", null)).IsSuccessStatusCode; }
        catch { return false; }
    }

    public async Task<bool> SetNicknameNullAsync(int id)
    {
        try
        {
            var response = await _http.PutAsJsonAsync($"api/admin/users/{id}/nickname", new AdminSetNicknameDto { Nickname = null });
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    // ── Reviews ────────────────────────────────────────────────────
    public async Task<List<ReviewDto>> GetUserReviewsAsync(int userId)
    {
        try { return await _http.GetFromJsonAsync<List<ReviewDto>>($"api/admin/users/{userId}/reviews") ?? []; }
        catch { return []; }
    }

    public async Task<bool> RemoveReviewCommentAsync(int reviewId)
    {
        try { return (await _http.DeleteAsync($"api/admin/reviews/{reviewId}/comment")).IsSuccessStatusCode; }
        catch { return false; }
    }

    public async Task<bool> DeleteReviewAsync(int reviewId)
    {
        try { return (await _http.DeleteAsync($"api/admin/reviews/{reviewId}")).IsSuccessStatusCode; }
        catch { return false; }
    }

    // ── Rentals ────────────────────────────────────────────────────
    public async Task<List<RentalDto>> GetUserRentalsAsync(int userId)
    {
        try { return await _http.GetFromJsonAsync<List<RentalDto>>($"api/admin/users/{userId}/rentals") ?? []; }
        catch { return []; }
    }

    public async Task<bool> CancelRentalAsync(int rentalId)
    {
        try { return (await _http.PutAsync($"api/admin/rentals/{rentalId}/cancel", null)).IsSuccessStatusCode; }
        catch { return false; }
    }

    // ── Movies ─────────────────────────────────────────────────────
    public async Task<List<MovieListDto>> GetAllMoviesAsync()
    {
        try { return await _http.GetFromJsonAsync<List<MovieListDto>>("api/admin/movies") ?? []; }
        catch { return []; }
    }

    public async Task<MovieDetailsDto?> GetMovieByIdAsync(int id)
    {
        try { return await _http.GetFromJsonAsync<MovieDetailsDto>($"api/admin/movies/{id}"); }
        catch { return null; }
    }

    public async Task<MovieDetailsDto?> CreateMovieAsync(CreateMovieDto dto)
    {
        try
        {
            var r = await _http.PostAsJsonAsync("api/admin/movies", dto);
            return r.IsSuccessStatusCode ? await r.Content.ReadFromJsonAsync<MovieDetailsDto>() : null;
        }
        catch { return null; }
    }

    public async Task<MovieDetailsDto?> UpdateMovieAsync(int id, UpdateMovieDto dto)
    {
        try
        {
            var r = await _http.PutAsJsonAsync($"api/admin/movies/{id}", dto);
            return r.IsSuccessStatusCode ? await r.Content.ReadFromJsonAsync<MovieDetailsDto>() : null;
        }
        catch { return null; }
    }

    public async Task<bool> DeleteMovieAsync(int id)
    {
        try { return (await _http.DeleteAsync($"api/admin/movies/{id}")).IsSuccessStatusCode; }
        catch { return false; }
    }

    // ── Reference data ─────────────────────────────────────────────
    public async Task<List<GenreDto>> GetGenresAsync()
    {
        try { return await _http.GetFromJsonAsync<List<GenreDto>>("api/genres") ?? []; }
        catch { return []; }
    }

    public async Task<List<PersonDto>> GetPersonsAsync(string? search = null)
    {
        try
        {
            var url = string.IsNullOrWhiteSpace(search) ? "api/persons" : $"api/persons?search={Uri.EscapeDataString(search)}";
            return await _http.GetFromJsonAsync<List<PersonDto>>(url) ?? [];
        }
        catch { return []; }
    }

    public async Task<List<ProductionCompanyDto>> GetProductionCompaniesAsync()
    {
        try { return await _http.GetFromJsonAsync<List<ProductionCompanyDto>>("api/production-companies") ?? []; }
        catch { return []; }
    }
}
```

- [ ] **Step 3: Register in Program.cs**

In `RetroVHS.Client/Program.cs`, after the `IUserClient` line, add:
```csharp
builder.Services.AddScoped<IAdminClient, AdminClient>();
```

- [ ] **Step 4: Commit**

```bash
git add RetroVHS.Client/Services/IAdminClient.cs RetroVHS.Client/Services/AdminClient.cs RetroVHS.Client/Program.cs
git commit -m "feat: add IAdminClient/AdminClient and register in DI"
```

---

## Task 9: Header — Admin Link

**Files:**
- Modify: `RetroVHS.Client/Components/Layout/Header.razor`

- [ ] **Step 1: Add admin link inside the authorized dropdown**

In `Header.razor`, inside the `<Authorized>` block, locate the `<div class="user-menu-dropdown">`. Find the first `<div class="user-menu-divider"></div>` (before logout). Just before it, add:

```razor
<AuthorizeView Roles="Admin" Context="adminCtx">
    <Authorized>
        <div class="user-menu-divider"></div>
        <a class="user-menu-item" href="/admin" @onclick="CloseUserMenu">
            <svg xmlns="http://www.w3.org/2000/svg" width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                <path d="M12 22s8-4 8-10V5l-8-3-8 3v7c0 6 8 10 8 10z"/>
            </svg>
            Admin
        </a>
    </Authorized>
</AuthorizeView>
```

- [ ] **Step 2: Commit**

```bash
git add RetroVHS.Client/Components/Layout/Header.razor
git commit -m "feat: add admin link in header dropdown for Admin role"
```

---

## Task 10: Admin Dashboard

**Files:**
- Create: `RetroVHS.Client/Components/Pages/Admin/Dashboard.razor`

- [ ] **Step 1: Create Dashboard.razor**

```razor
@page "/admin"
@attribute [Authorize(Roles = "Admin")]
@using RetroVHS.Shared.DTOs.Admin
@using Microsoft.AspNetCore.Authorization
@inject IAdminClient AdminClient
@inject NavigationManager Navigation

<PageTitle>Admin — Dashboard</PageTitle>

<div class="admin-page">
    <h1 class="section-title">🛡️ Admin Dashboard</h1>

    @if (isLoading)
    {
        <div class="loading-spinner">
            <div class="spinner-border" role="status" style="color:#e94560;width:2.5rem;height:2.5rem;"></div>
            <p>Laddar statistik...</p>
        </div>
    }
    else if (stats is null)
    {
        <p class="review-error">Kunde inte hämta statistik.</p>
    }
    else
    {
        <div class="admin-stat-grid">
            <div class="admin-stat-card">
                <div class="admin-stat-value">@stats.TotalUsers</div>
                <div class="admin-stat-label">Användare</div>
            </div>
            <div class="admin-stat-card">
                <div class="admin-stat-value">@stats.BlockedUsers</div>
                <div class="admin-stat-label">Blockerade</div>
            </div>
            <div class="admin-stat-card">
                <div class="admin-stat-value">@stats.TotalMovies</div>
                <div class="admin-stat-label">Filmer</div>
            </div>
            <div class="admin-stat-card">
                <div class="admin-stat-value">@stats.ActiveRentals</div>
                <div class="admin-stat-label">Aktiva uthyrningar</div>
            </div>
            <div class="admin-stat-card">
                <div class="admin-stat-value">@stats.TotalRentals</div>
                <div class="admin-stat-label">Totala uthyrningar</div>
            </div>
            <div class="admin-stat-card">
                <div class="admin-stat-value">@stats.TotalReviews</div>
                <div class="admin-stat-label">Recensioner</div>
            </div>
        </div>

        <div class="admin-nav-links">
            <a href="/admin/users">👥 Hantera användare</a>
            <a href="/admin/movies">🎬 Hantera filmer</a>
        </div>
    }
</div>

@code {
    private AdminDashboardDto? stats;
    private bool isLoading = true;

    protected override async Task OnInitializedAsync()
    {
        stats = await AdminClient.GetDashboardStatsAsync();
        isLoading = false;
    }
}
```

- [ ] **Step 2: Commit**

```bash
git add RetroVHS.Client/Components/Pages/Admin/Dashboard.razor
git commit -m "feat(admin): add admin dashboard page with stat cards"
```

---

## Task 11: Admin User List

**Files:**
- Create: `RetroVHS.Client/Components/Pages/Admin/Users.razor`

- [ ] **Step 1: Create Users.razor**

```razor
@page "/admin/users"
@attribute [Authorize(Roles = "Admin")]
@using RetroVHS.Shared.DTOs.Auth
@using Microsoft.AspNetCore.Authorization
@inject IAdminClient AdminClient
@inject NavigationManager Navigation

<PageTitle>Admin — Användare</PageTitle>

<div class="admin-page">
    <a href="/admin" class="admin-back-link">← Dashboard</a>
    <h1 class="section-title">👥 Användare</h1>

    @if (isLoading)
    {
        <div class="loading-spinner">
            <div class="spinner-border" role="status" style="color:#e94560;width:2.5rem;height:2.5rem;"></div>
            <p>Laddar användare...</p>
        </div>
    }
    else if (users.Count == 0)
    {
        <p style="color:#a0a0b0;">Inga användare hittades.</p>
    }
    else
    {
        <table class="admin-table">
            <thead>
                <tr>
                    <th>Namn</th>
                    <th>E-post</th>
                    <th>Nickname</th>
                    <th>Status</th>
                    <th>Åtgärder</th>
                </tr>
            </thead>
            <tbody>
                @foreach (var user in users)
                {
                    <tr>
                        <td>@user.FullName</td>
                        <td>@user.Email</td>
                        <td>@(user.Nickname ?? "—")</td>
                        <td>
                            @if (user.IsBlocked)
                            {
                                <span class="rental-status status-cancelled">Blockerad</span>
                            }
                            else
                            {
                                <span class="rental-status status-completed">Aktiv</span>
                            }
                        </td>
                        <td>
                            <div class="action-links">
                                <a href="/admin/users/@user.Id">Profil</a>
                                <a href="/admin/users/@user.Id/reviews">Recensioner</a>
                                <a href="/admin/users/@user.Id/orders">Beställningar</a>
                                @if (confirmDeleteId == user.Id)
                                {
                                    <div class="confirm-inline" style="margin-top:0.3rem;">
                                        <p>Radera @user.FullName?</p>
                                        <button class="btn-confirm-yes" @onclick="() => DeleteUserAsync(user.Id)" disabled="@deleting">
                                            @(deleting ? "..." : "Ja, radera")
                                        </button>
                                        <button class="btn-confirm-cancel" @onclick="() => confirmDeleteId = null" disabled="@deleting">Avbryt</button>
                                    </div>
                                }
                                else
                                {
                                    <button class="btn-remove" style="font-size:0.8rem;padding:3px 10px;"
                                            @onclick="() => confirmDeleteId = user.Id">Radera</button>
                                }
                            </div>
                        </td>
                    </tr>
                }
            </tbody>
        </table>
    }
</div>

@code {
    private List<UserDto> users = [];
    private bool isLoading = true;
    private int? confirmDeleteId;
    private bool deleting;

    protected override async Task OnInitializedAsync()
    {
        users = await AdminClient.GetAllUsersAsync();
        isLoading = false;
    }

    private async Task DeleteUserAsync(int id)
    {
        deleting = true;
        var success = await AdminClient.DeleteUserAsync(id);
        if (success) users.RemoveAll(u => u.Id == id);
        confirmDeleteId = null;
        deleting = false;
    }
}
```

- [ ] **Step 2: Commit**

```bash
git add RetroVHS.Client/Components/Pages/Admin/Users.razor
git commit -m "feat(admin): add admin user list page"
```

---

## Task 12: Admin User Profile

**Files:**
- Create: `RetroVHS.Client/Components/Pages/Admin/UserProfile.razor`

- [ ] **Step 1: Create UserProfile.razor**

```razor
@page "/admin/users/{Id:int}"
@attribute [Authorize(Roles = "Admin")]
@using RetroVHS.Shared.DTOs.Auth
@using Microsoft.AspNetCore.Authorization
@inject IAdminClient AdminClient
@inject NavigationManager Navigation

<PageTitle>Admin — Användarprofil</PageTitle>

<div class="admin-page">
    <a href="/admin/users" class="admin-back-link">← Alla användare</a>

    @if (isLoading)
    {
        <div class="loading-spinner">
            <div class="spinner-border" role="status" style="color:#e94560;width:2.5rem;height:2.5rem;"></div>
            <p>Laddar...</p>
        </div>
    }
    else if (user is null)
    {
        <p class="review-error">Användaren hittades inte.</p>
    }
    else
    {
        <h1 class="section-title">@user.FullName</h1>

        <div class="admin-nav-links" style="margin-bottom:1.5rem;">
            <a href="/admin/users/@user.Id/reviews">Recensioner</a>
            <a href="/admin/users/@user.Id/orders">Beställningar</a>
        </div>

        <div class="admin-user-info">
            <p><strong>E-post:</strong> @user.Email</p>
            <p><strong>Nickname:</strong> @(user.Nickname ?? "—")</p>
            <p><strong>Status:</strong>
                @if (user.IsBlocked)
                {
                    <span class="rental-status status-cancelled" style="margin-left:0.4rem;">Blockerad</span>
                }
                else
                {
                    <span class="rental-status status-completed" style="margin-left:0.4rem;">Aktiv</span>
                }
            </p>
        </div>

        <div class="admin-actions">
            @* ── Nickname null ── *@
            @if (showNicknameConfirm)
            {
                <div class="confirm-inline">
                    <p>Sätt nickname till null för @user.FullName?</p>
                    <button class="btn-confirm-yes" @onclick="SetNicknameNullAsync" disabled="@acting">@(acting ? "..." : "Ja")</button>
                    <button class="btn-confirm-cancel" @onclick="() => showNicknameConfirm = false" disabled="@acting">Avbryt</button>
                </div>
            }
            else
            {
                <button class="btn-submit" style="max-width:240px;" @onclick="() => showNicknameConfirm = true" disabled="@(user.Nickname is null)">
                    Sätt nickname till null
                </button>
            }

            @* ── Block / Unblock ── *@
            @if (showBlockConfirm)
            {
                <div class="confirm-inline">
                    <p>@(user.IsBlocked ? "Avblockera" : "Blockera") @user.FullName?</p>
                    <button class="btn-confirm-yes" @onclick="ToggleBlockAsync" disabled="@acting">@(acting ? "..." : "Ja")</button>
                    <button class="btn-confirm-cancel" @onclick="() => showBlockConfirm = false" disabled="@acting">Avbryt</button>
                </div>
            }
            else
            {
                <button class="btn-submit" style="max-width:240px;" @onclick="() => showBlockConfirm = true">
                    @(user.IsBlocked ? "Avblockera användare" : "Blockera användare")
                </button>
            }

            @* ── Delete ── *@
            @if (showDeleteConfirm)
            {
                <div class="confirm-inline">
                    <p>Radera @user.FullName och all data permanent?</p>
                    <button class="btn-confirm-yes" @onclick="DeleteUserAsync" disabled="@acting">@(acting ? "..." : "Ja, radera")</button>
                    <button class="btn-confirm-cancel" @onclick="() => showDeleteConfirm = false" disabled="@acting">Avbryt</button>
                </div>
            }
            else
            {
                <button class="btn-remove" style="max-width:240px;" @onclick="() => showDeleteConfirm = true">Radera användare</button>
            }

            @if (!string.IsNullOrEmpty(actionMessage))
            {
                <p class="@(actionSuccess ? "profile-success" : "review-error")">@actionMessage</p>
            }
        </div>
    }
</div>

@code {
    [Parameter] public int Id { get; set; }
    private UserDto? user;
    private bool isLoading = true;
    private bool acting;
    private bool showNicknameConfirm;
    private bool showBlockConfirm;
    private bool showDeleteConfirm;
    private string? actionMessage;
    private bool actionSuccess;

    protected override async Task OnInitializedAsync()
    {
        user = await AdminClient.GetUserByIdAsync(Id);
        isLoading = false;
    }

    private async Task SetNicknameNullAsync()
    {
        acting = true;
        var ok = await AdminClient.SetNicknameNullAsync(Id);
        if (ok && user is not null) user.Nickname = null;
        showNicknameConfirm = false;
        actionMessage = ok ? "Nickname borttaget." : "Åtgärden misslyckades.";
        actionSuccess = ok;
        acting = false;
    }

    private async Task ToggleBlockAsync()
    {
        acting = true;
        var ok = user!.IsBlocked
            ? await AdminClient.UnblockUserAsync(Id)
            : await AdminClient.BlockUserAsync(Id);
        if (ok) user.IsBlocked = !user.IsBlocked;
        showBlockConfirm = false;
        actionMessage = ok ? (user.IsBlocked ? "Användaren blockerad." : "Användaren avblockerad.") : "Åtgärden misslyckades.";
        actionSuccess = ok;
        acting = false;
    }

    private async Task DeleteUserAsync()
    {
        acting = true;
        var ok = await AdminClient.DeleteUserAsync(Id);
        acting = false;
        if (ok) Navigation.NavigateTo("/admin/users");
        else { actionMessage = "Kunde inte radera användaren."; actionSuccess = false; showDeleteConfirm = false; }
    }
}
```

- [ ] **Step 2: Commit**

```bash
git add RetroVHS.Client/Components/Pages/Admin/UserProfile.razor
git commit -m "feat(admin): add admin user profile page"
```

---

## Task 13: Admin User Reviews

**Files:**
- Create: `RetroVHS.Client/Components/Pages/Admin/UserReviews.razor`

- [ ] **Step 1: Create UserReviews.razor**

```razor
@page "/admin/users/{UserId:int}/reviews"
@attribute [Authorize(Roles = "Admin")]
@using RetroVHS.Shared.DTOs.Reviews
@using Microsoft.AspNetCore.Authorization
@inject IAdminClient AdminClient

<PageTitle>Admin — Recensioner</PageTitle>

<div class="admin-page">
    <a href="/admin/users/@UserId" class="admin-back-link">← Användarprofil</a>
    <h1 class="section-title">Recensioner</h1>

    @if (isLoading)
    {
        <div class="loading-spinner">
            <div class="spinner-border" role="status" style="color:#e94560;width:2.5rem;height:2.5rem;"></div>
            <p>Laddar recensioner...</p>
        </div>
    }
    else if (reviews.Count == 0)
    {
        <p style="color:#a0a0b0;">Inga recensioner hittades.</p>
    }
    else
    {
        <div class="cart-items">
            @foreach (var review in reviews)
            {
                <div class="cart-item">
                    <div class="cart-item-info">
                        <h3><a href="/movie/@review.MovieId" style="color:#e0e0f0;text-decoration:none;">@review.MovieTitle</a></h3>
                        <p style="color:#f5c518;">@(new string('★', review.Rating))@(new string('☆', 5 - review.Rating)) @review.Rating/5</p>
                        <p style="color:#a0a0b0;font-size:0.82rem;">@review.CreatedAt.ToString("d MMM yyyy")</p>
                        @if (!string.IsNullOrWhiteSpace(review.Comment))
                        {
                            <p class="review-comment">@review.Comment</p>
                        }
                        else
                        {
                            <p style="color:#606070;font-size:0.85rem;font-style:italic;">Ingen kommentar</p>
                        }

                        <div style="display:flex;gap:0.5rem;flex-wrap:wrap;margin-top:0.5rem;">
                            @if (confirmRemoveCommentId == review.Id)
                            {
                                <div class="confirm-inline">
                                    <p>Ta bort kommentaren? Betyget behålls.</p>
                                    <button class="btn-confirm-yes" @onclick="() => RemoveCommentAsync(review)" disabled="@acting">@(acting ? "..." : "Ja")</button>
                                    <button class="btn-confirm-cancel" @onclick="() => confirmRemoveCommentId = null" disabled="@acting">Avbryt</button>
                                </div>
                            }
                            else if (!string.IsNullOrWhiteSpace(review.Comment))
                            {
                                <button class="btn-submit" style="font-size:0.82rem;padding:5px 12px;"
                                        @onclick="() => confirmRemoveCommentId = review.Id">Ta bort kommentar</button>
                            }

                            @if (confirmDeleteId == review.Id)
                            {
                                <div class="confirm-inline">
                                    <p>Radera hela recensionen?</p>
                                    <button class="btn-confirm-yes" @onclick="() => DeleteReviewAsync(review.Id)" disabled="@acting">@(acting ? "..." : "Ja, radera")</button>
                                    <button class="btn-confirm-cancel" @onclick="() => confirmDeleteId = null" disabled="@acting">Avbryt</button>
                                </div>
                            }
                            else
                            {
                                <button class="btn-remove" style="font-size:0.82rem;padding:5px 12px;"
                                        @onclick="() => confirmDeleteId = review.Id">Radera recension</button>
                            }
                        </div>
                    </div>
                </div>
            }
        </div>
    }
</div>

@code {
    [Parameter] public int UserId { get; set; }
    private List<ReviewDto> reviews = [];
    private bool isLoading = true;
    private int? confirmRemoveCommentId;
    private int? confirmDeleteId;
    private bool acting;

    protected override async Task OnInitializedAsync()
    {
        reviews = await AdminClient.GetUserReviewsAsync(UserId);
        isLoading = false;
    }

    private async Task RemoveCommentAsync(ReviewDto review)
    {
        acting = true;
        var ok = await AdminClient.RemoveReviewCommentAsync(review.Id);
        if (ok) review.Comment = string.Empty;
        confirmRemoveCommentId = null;
        acting = false;
    }

    private async Task DeleteReviewAsync(int reviewId)
    {
        acting = true;
        var ok = await AdminClient.DeleteReviewAsync(reviewId);
        if (ok) reviews.RemoveAll(r => r.Id == reviewId);
        confirmDeleteId = null;
        acting = false;
    }
}
```

- [ ] **Step 2: Commit**

```bash
git add RetroVHS.Client/Components/Pages/Admin/UserReviews.razor
git commit -m "feat(admin): add admin user reviews page"
```

---

## Task 14: Admin User Orders

**Files:**
- Create: `RetroVHS.Client/Components/Pages/Admin/UserOrders.razor`

- [ ] **Step 1: Create UserOrders.razor**

```razor
@page "/admin/users/{UserId:int}/orders"
@attribute [Authorize(Roles = "Admin")]
@using RetroVHS.Shared.DTOs.Rentals
@using RetroVHS.Shared.Enums
@using Microsoft.AspNetCore.Authorization
@inject IAdminClient AdminClient

<PageTitle>Admin — Beställningar</PageTitle>

<div class="admin-page">
    <a href="/admin/users/@UserId" class="admin-back-link">← Användarprofil</a>
    <h1 class="section-title">Beställningar</h1>

    @if (isLoading)
    {
        <div class="loading-spinner">
            <div class="spinner-border" role="status" style="color:#e94560;width:2.5rem;height:2.5rem;"></div>
            <p>Laddar beställningar...</p>
        </div>
    }
    else if (rentals.Count == 0)
    {
        <p style="color:#a0a0b0;">Inga beställningar hittades.</p>
    }
    else
    {
        <div class="cart-items">
            @foreach (var rental in rentals)
            {
                <div class="cart-item">
                    <div class="cart-item-info">
                        <h3>@rental.Title</h3>
                        <p class="cart-item-price">@rental.PricePaid.ToString("0.00") kr</p>
                        <p style="color:#a0a0b0;font-size:0.82rem;">@rental.RentedAt.ToString("d MMM yyyy")</p>
                        <div style="margin-top:6px;">
                            <span class="rental-status @GetStatusClass(rental.Status)">@GetStatusLabel(rental.Status)</span>
                        </div>

                        @if (rental.Status == RentalStatus.Active)
                        {
                            @if (confirmCancelId == rental.Id)
                            {
                                <div class="confirm-inline">
                                    <p>Avbryt denna beställning?</p>
                                    <button class="btn-confirm-yes" @onclick="() => CancelRentalAsync(rental)" disabled="@acting">@(acting ? "..." : "Ja, avbryt")</button>
                                    <button class="btn-confirm-cancel" @onclick="() => confirmCancelId = null" disabled="@acting">Nej</button>
                                </div>
                            }
                            else
                            {
                                <button class="btn-remove" style="font-size:0.82rem;padding:5px 12px;margin-top:0.5rem;"
                                        @onclick="() => confirmCancelId = rental.Id">Avbryt beställning</button>
                            }
                        }
                    </div>
                </div>
            }
        </div>
    }
</div>

@code {
    [Parameter] public int UserId { get; set; }
    private List<RentalDto> rentals = [];
    private bool isLoading = true;
    private int? confirmCancelId;
    private bool acting;

    protected override async Task OnInitializedAsync()
    {
        rentals = await AdminClient.GetUserRentalsAsync(UserId);
        isLoading = false;
    }

    private async Task CancelRentalAsync(RentalDto rental)
    {
        acting = true;
        var ok = await AdminClient.CancelRentalAsync(rental.Id);
        if (ok) rental.Status = RentalStatus.Cancelled;
        confirmCancelId = null;
        acting = false;
    }

    private static string GetStatusClass(RentalStatus s) => s switch
    {
        RentalStatus.Active    => "status-active",
        RentalStatus.Completed => "status-completed",
        RentalStatus.Cancelled => "status-cancelled",
        RentalStatus.Expired   => "status-expired",
        _ => ""
    };

    private static string GetStatusLabel(RentalStatus s) => s switch
    {
        RentalStatus.Active    => "Aktiv",
        RentalStatus.Completed => "Levererad",
        RentalStatus.Cancelled => "Avbruten",
        RentalStatus.Expired   => "Utgången",
        _ => s.ToString()
    };
}
```

- [ ] **Step 2: Commit**

```bash
git add RetroVHS.Client/Components/Pages/Admin/UserOrders.razor
git commit -m "feat(admin): add admin user orders page"
```

---

## Task 15: Admin Movie List

**Files:**
- Create: `RetroVHS.Client/Components/Pages/Admin/MovieList.razor`

- [ ] **Step 1: Create MovieList.razor**

```razor
@page "/admin/movies"
@attribute [Authorize(Roles = "Admin")]
@using RetroVHS.Shared.DTOs.Movies
@using Microsoft.AspNetCore.Authorization
@inject IAdminClient AdminClient
@inject NavigationManager Navigation

<PageTitle>Admin — Filmer</PageTitle>

<div class="admin-page">
    <a href="/admin" class="admin-back-link">← Dashboard</a>
    <div style="display:flex;justify-content:space-between;align-items:center;flex-wrap:wrap;gap:0.5rem;margin-bottom:1rem;">
        <h1 class="section-title" style="margin:0;">🎬 Filmer</h1>
        <a href="/admin/movies/new" class="btn-submit" style="text-decoration:none;padding:8px 16px;font-size:0.9rem;">+ Lägg till film</a>
    </div>

    @if (isLoading)
    {
        <div class="loading-spinner">
            <div class="spinner-border" role="status" style="color:#e94560;width:2.5rem;height:2.5rem;"></div>
            <p>Laddar filmer...</p>
        </div>
    }
    else if (movies.Count == 0)
    {
        <p style="color:#a0a0b0;">Inga filmer hittades.</p>
    }
    else
    {
        <div class="movies-list">
            @foreach (var movie in movies)
            {
                <div class="movie-list-row">
                    <span class="movie-list-year">@movie.ReleaseYear</span>
                    <a class="movie-list-title" href="/movie/@movie.Id">@movie.Title</a>
                    @if (movie.RatingCount > 0)
                    {
                        <span class="movie-list-rating">★ @movie.RatingAverage.ToString("0.0")</span>
                    }
                    else
                    {
                        <span class="movie-list-rating no-rating">Inga betyg</span>
                    }
                    <div class="movie-list-actions">
                        <a href="/admin/movies/@movie.Id/edit">Redigera</a>
                        @if (confirmDeleteId == movie.Id)
                        {
                            <div class="confirm-inline" style="position:static;margin-top:0.3rem;">
                                <p>Radera "@movie.Title"?</p>
                                <button class="btn-confirm-yes" @onclick="() => DeleteMovieAsync(movie.Id)" disabled="@deleting">@(deleting ? "..." : "Ja")</button>
                                <button class="btn-confirm-cancel" @onclick="() => confirmDeleteId = null" disabled="@deleting">Avbryt</button>
                            </div>
                        }
                        else
                        {
                            <button class="btn-remove" style="font-size:0.8rem;padding:3px 10px;border:none;background:transparent;cursor:pointer;"
                                    @onclick="() => confirmDeleteId = movie.Id">Radera</button>
                        }
                    </div>
                </div>
            }
        </div>
    }
</div>

@code {
    private List<MovieListDto> movies = [];
    private bool isLoading = true;
    private int? confirmDeleteId;
    private bool deleting;

    protected override async Task OnInitializedAsync()
    {
        movies = await AdminClient.GetAllMoviesAsync();
        isLoading = false;
    }

    private async Task DeleteMovieAsync(int id)
    {
        deleting = true;
        var ok = await AdminClient.DeleteMovieAsync(id);
        if (ok) movies.RemoveAll(m => m.Id == id);
        confirmDeleteId = null;
        deleting = false;
    }
}
```

- [ ] **Step 2: Commit**

```bash
git add RetroVHS.Client/Components/Pages/Admin/MovieList.razor
git commit -m "feat(admin): add admin movie list page"
```

---

## Task 16: Admin Movie Form

**Files:**
- Create: `RetroVHS.Client/Components/Pages/Admin/MovieForm.razor`

This is the most complex component. It handles both create (`/admin/movies/new`) and edit (`/admin/movies/{Id}/edit`), and includes dynamic person-search rows for directors and cast.

- [ ] **Step 1: Create MovieForm.razor**

```razor
@page "/admin/movies/new"
@page "/admin/movies/{Id:int}/edit"
@attribute [Authorize(Roles = "Admin")]
@using RetroVHS.Shared.DTOs.Movies
@using RetroVHS.Shared.DTOs.Genres
@using RetroVHS.Shared.DTOs.Persons
@using RetroVHS.Shared.DTOs.ProductionCompanies
@using RetroVHS.Shared.Enums
@using Microsoft.AspNetCore.Authorization
@inject IAdminClient AdminClient
@inject NavigationManager Navigation

<PageTitle>@(Id.HasValue ? "Redigera film" : "Lägg till film")</PageTitle>

<div class="movie-form-page">
    <a href="/admin/movies" class="admin-back-link">← Filmlistan</a>
    <h1 class="section-title">@(Id.HasValue ? "✏️ Redigera film" : "➕ Lägg till film")</h1>

    @if (isLoading)
    {
        <div class="loading-spinner">
            <div class="spinner-border" role="status" style="color:#e94560;width:2.5rem;height:2.5rem;"></div>
            <p>Laddar...</p>
        </div>
    }
    else
    {
        <div class="movie-form-grid">

            @* ── Grundfält ── *@
            <div class="movie-form-field">
                <label>Titel *</label>
                <input type="text" @bind="title" placeholder="Filmens titel" />
                @if (showErrors && string.IsNullOrWhiteSpace(title))
                { <span class="validation-message">Titel krävs.</span> }
            </div>

            <div class="movie-form-field">
                <label>Utgivningsår *</label>
                <input type="number" @bind="releaseYear" min="1910" max="2100" />
                @if (showErrors && (releaseYear < 1910 || releaseYear > 2100))
                { <span class="validation-message">Ange ett giltigt år (1910–2100).</span> }
            </div>

            <div class="movie-form-field">
                <label>Längd (minuter) *</label>
                <input type="number" @bind="durationMinutes" min="1" max="600" />
            </div>

            <div class="movie-form-field">
                <label>Hyrespris (kr) *</label>
                <input type="number" @bind="rentalPrice" min="0.01" max="999.99" step="0.01" />
            </div>

            <div class="movie-form-field">
                <label>Lagersaldo</label>
                <input type="number" @bind="stockQuantity" min="0" max="10000" />
            </div>

            <div class="movie-form-field">
                <label>Tillgänglighetsstatus</label>
                <select @bind="availabilityStatus">
                    @foreach (var s in Enum.GetValues<MovieAvailabilityStatus>())
                    {
                        <option value="@s">@s</option>
                    }
                </select>
            </div>

            <div class="movie-form-field">
                <label>Poster-URL</label>
                <input type="text" @bind="posterUrl" placeholder="https://..." />
            </div>

            <div class="movie-form-field">
                <label>Trailer-URL (YouTube)</label>
                <input type="text" @bind="trailerUrl" placeholder="https://youtube.com/..." />
            </div>

            <div class="movie-form-field">
                <label>Språk</label>
                <input type="text" @bind="language" placeholder="t.ex. English" />
            </div>

            <div class="movie-form-field">
                <label>Land</label>
                <input type="text" @bind="country" placeholder="t.ex. USA" />
            </div>

            <div class="movie-form-field full-width" style="flex-direction:row;align-items:center;gap:0.75rem;">
                <input type="checkbox" id="featured" @bind="isFeatured" />
                <label for="featured" style="margin:0;font-size:0.9rem;color:#e0e0f0;">Utvald (visas på startsidan)</label>
            </div>

            <div class="movie-form-field full-width">
                <label>Synopsis *</label>
                <textarea @bind="synopsis" rows="4" maxlength="1000" placeholder="Filmens beskrivning..."></textarea>
                @if (showErrors && string.IsNullOrWhiteSpace(synopsis))
                { <span class="validation-message">Synopsis krävs.</span> }
            </div>

            @* ── Produktionsbolag ── *@
            <div class="movie-form-field full-width">
                <label>Produktionsbolag</label>
                <select @bind="productionCompanyIdStr">
                    <option value="">— Inget —</option>
                    @foreach (var c in companies)
                    {
                        <option value="@c.Id">@c.Name</option>
                    }
                </select>
            </div>

            @* ── Genres ── *@
            <div class="movie-form-field full-width">
                <label>Genres</label>
                <div class="genre-tag-list">
                    @foreach (var g in allGenres)
                    {
                        <span class="genre-tag @(selectedGenreIds.Contains(g.Id) ? "selected" : "")"
                              @onclick="() => ToggleGenre(g.Id)">
                            @g.Name
                        </span>
                    }
                </div>
            </div>

            @* ── Regissörer ── *@
            <div class="movie-form-field full-width">
                <label>Regissörer</label>
                @for (int i = 0; i < directors.Count; i++)
                {
                    var idx = i;
                    <div class="person-search-row">
                        <div class="person-row-inputs">
                            <input type="text" placeholder="Sök person..."
                                   value="@directors[idx].DisplayName"
                                   @oninput="e => OnPersonSearchInput(e, idx, directors)"
                                   style="flex:1;" />
                            <button type="button" class="btn-remove-row" @onclick="() => directors.RemoveAt(idx)">✕</button>
                        </div>
                        @if (directors[idx].ShowDropdown && directors[idx].SearchResults.Count > 0)
                        {
                            <div class="person-search-result-dropdown">
                                @foreach (var p in directors[idx].SearchResults)
                                {
                                    var person = p;
                                    <div class="person-result-item" @onmousedown="() => SelectPerson(person, idx, directors)">@p.FullName</div>
                                }
                            </div>
                        }
                    </div>
                }
                <button type="button" class="btn-add-row" @onclick="() => directors.Add(new PersonRow())">+ Lägg till regissör</button>
            </div>

            @* ── Skådespelare ── *@
            <div class="movie-form-field full-width">
                <label>Skådespelare</label>
                @for (int i = 0; i < cast.Count; i++)
                {
                    var idx = i;
                    <div class="person-search-row">
                        <div class="person-row-inputs">
                            <input type="text" placeholder="Sök person..."
                                   value="@cast[idx].DisplayName"
                                   @oninput="e => OnPersonSearchInput(e, idx, cast)"
                                   style="flex:1;" />
                            <input type="text" placeholder="Rollnamn (valfritt)"
                                   @bind="cast[idx].CharacterName"
                                   style="flex:1;" />
                            <input type="number" placeholder="Ordning"
                                   @bind="cast[idx].DisplayOrder"
                                   style="width:80px;" min="0" />
                            <button type="button" class="btn-remove-row" @onclick="() => cast.RemoveAt(idx)">✕</button>
                        </div>
                        @if (cast[idx].ShowDropdown && cast[idx].SearchResults.Count > 0)
                        {
                            <div class="person-search-result-dropdown">
                                @foreach (var p in cast[idx].SearchResults)
                                {
                                    var person = p;
                                    <div class="person-result-item" @onmousedown="() => SelectPerson(person, idx, cast)">@p.FullName</div>
                                }
                            </div>
                        }
                    </div>
                }
                <button type="button" class="btn-add-row" @onclick="() => cast.Add(new PersonRow())">+ Lägg till skådespelare</button>
            </div>

        </div>

        @if (!string.IsNullOrEmpty(submitError))
        {
            <p class="review-error" style="margin-top:1rem;">@submitError</p>
        }

        <div style="margin-top:1.5rem;display:flex;gap:1rem;">
            <button class="btn-submit" @onclick="SubmitAsync" disabled="@submitting">
                @(submitting ? "Sparar..." : (Id.HasValue ? "Spara ändringar" : "Lägg till film"))
            </button>
            <a href="/admin/movies" class="btn-cancel-edit">Avbryt</a>
        </div>
    }
</div>

@code {
    [Parameter] public int? Id { get; set; }

    // Form fields
    private string title = "";
    private int releaseYear = DateTime.Now.Year;
    private int durationMinutes = 90;
    private string synopsis = "";
    private string? posterUrl;
    private string? trailerUrl;
    private string? language;
    private string? country;
    private decimal rentalPrice = 29.99m;
    private int stockQuantity = 0;
    private MovieAvailabilityStatus availabilityStatus = MovieAvailabilityStatus.Available;
    private bool isFeatured = false;
    private string productionCompanyIdStr = ""; // string to avoid Blazor int? bind issues

    private HashSet<int> selectedGenreIds = [];
    private List<PersonRow> directors = [];
    private List<PersonRow> cast = [];

    // Reference data
    private List<GenreDto> allGenres = [];
    private List<ProductionCompanyDto> companies = [];

    private bool isLoading = true;
    private bool submitting;
    private bool showErrors;
    private string? submitError;

    private CancellationTokenSource? _debounce;

    protected override async Task OnInitializedAsync()
    {
        var genresTask = AdminClient.GetGenresAsync();
        var companiesTask = AdminClient.GetProductionCompaniesAsync();
        allGenres = await genresTask;
        companies = await companiesTask;

        if (Id.HasValue)
        {
            var movie = await AdminClient.GetMovieByIdAsync(Id.Value);
            if (movie is not null)
            {
                title = movie.Title;
                releaseYear = movie.ReleaseYear;
                durationMinutes = movie.DurationMinutes;
                synopsis = movie.Synopsis ?? "";
                posterUrl = movie.PosterUrl;
                trailerUrl = movie.TrailerUrl;
                rentalPrice = movie.RentalPrice;
                availabilityStatus = Enum.TryParse<MovieAvailabilityStatus>(movie.AvailabilityStatus, out var s) ? s : MovieAvailabilityStatus.Available;
                isFeatured = movie.IsFeatured;
                language = movie.Language;
                country = movie.Country;
                productionCompanyIdStr = movie.ProductionCompanyId?.ToString() ?? "";
                // Map genre names → IDs
                selectedGenreIds = allGenres
                    .Where(g => movie.Genres.Contains(g.Name))
                    .Select(g => g.Id)
                    .ToHashSet();
                // Map directors
                directors = movie.Directors.Select(d => new PersonRow { PersonId = d.PersonId, DisplayName = d.FullName }).ToList();
                // Map cast
                cast = movie.Cast.Select(c => new PersonRow { PersonId = c.PersonId, DisplayName = c.FullName, CharacterName = c.CharacterName, DisplayOrder = c.DisplayOrder }).ToList();
            }
        }

        isLoading = false;
    }

    private void ToggleGenre(int genreId)
    {
        if (!selectedGenreIds.Add(genreId))
            selectedGenreIds.Remove(genreId);
    }

    private async Task OnPersonSearchInput(ChangeEventArgs e, int idx, List<PersonRow> rows)
    {
        var term = e.Value?.ToString() ?? "";
        rows[idx].DisplayName = term;
        rows[idx].PersonId = null;

        _debounce?.Cancel();
        _debounce = new CancellationTokenSource();
        try
        {
            await Task.Delay(300, _debounce.Token);
            if (!string.IsNullOrWhiteSpace(term))
            {
                rows[idx].SearchResults = await AdminClient.GetPersonsAsync(term);
                rows[idx].ShowDropdown = true;
            }
            else
            {
                rows[idx].SearchResults = [];
                rows[idx].ShowDropdown = false;
            }
        }
        catch (TaskCanceledException) { }
    }

    private void SelectPerson(PersonDto person, int idx, List<PersonRow> rows)
    {
        rows[idx].PersonId = person.Id;
        rows[idx].DisplayName = person.FullName;
        rows[idx].ShowDropdown = false;
        rows[idx].SearchResults = [];
    }

    private async Task SubmitAsync()
    {
        showErrors = true;
        if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(synopsis)) return;

        submitting = true;
        submitError = null;

        var credits = new List<CreateMovieCreditDto>();
        foreach (var d in directors.Where(d => d.PersonId.HasValue))
            credits.Add(new CreateMovieCreditDto { PersonId = d.PersonId!.Value, Role = CreditRole.Director });
        foreach (var c in cast.Where(c => c.PersonId.HasValue))
            credits.Add(new CreateMovieCreditDto { PersonId = c.PersonId!.Value, Role = CreditRole.Actor, CharacterName = c.CharacterName, DisplayOrder = c.DisplayOrder });

        int? companyId = int.TryParse(productionCompanyIdStr, out var cid) && cid > 0 ? cid : null;

        if (Id.HasValue)
        {
            var dto = new UpdateMovieDto
            {
                Id = Id.Value, Title = title, Synopsis = synopsis, ReleaseYear = releaseYear,
                DurationMinutes = durationMinutes, RentalPrice = rentalPrice, PosterUrl = posterUrl,
                TrailerUrl = trailerUrl, Language = language, Country = country,
                ProductionCompanyId = companyId, AvailabilityStatus = availabilityStatus,
                StockQuantity = stockQuantity, IsFeatured = isFeatured,
                GenreIds = selectedGenreIds.ToList(), Credits = credits
            };
            var result = await AdminClient.UpdateMovieAsync(Id.Value, dto);
            if (result is not null) Navigation.NavigateTo("/admin/movies");
            else submitError = "Kunde inte uppdatera filmen. Kontrollera att alla ID:n är giltiga.";
        }
        else
        {
            var dto = new CreateMovieDto
            {
                Title = title, Synopsis = synopsis, ReleaseYear = releaseYear,
                DurationMinutes = durationMinutes, RentalPrice = rentalPrice, PosterUrl = posterUrl,
                TrailerUrl = trailerUrl, Language = language, Country = country,
                ProductionCompanyId = companyId, AvailabilityStatus = availabilityStatus,
                StockQuantity = stockQuantity, IsFeatured = isFeatured,
                GenreIds = selectedGenreIds.ToList(), Credits = credits
            };
            var result = await AdminClient.CreateMovieAsync(dto);
            if (result is not null) Navigation.NavigateTo("/admin/movies");
            else submitError = "Kunde inte skapa filmen. Kontrollera att alla ID:n är giltiga.";
        }

        submitting = false;
    }

    private class PersonRow
    {
        public int? PersonId { get; set; }
        public string DisplayName { get; set; } = "";
        public string? CharacterName { get; set; }
        public int DisplayOrder { get; set; } = 0;
        public bool ShowDropdown { get; set; }
        public List<PersonDto> SearchResults { get; set; } = [];
    }
}
```

**Note:** `MovieDetailsDto` must have `PersonId` on each `PersonCreditDto` for the edit pre-fill to work. Verify `PersonCreditDto.PersonId` exists in `RetroVHS.Shared/DTOs/Movies/PersonCreditDto.cs`. If missing, add `public int PersonId { get; set; }` and ensure the API returns it.

- [ ] **Step 2: Verify PersonCreditDto has PersonId**

Read `RetroVHS.Shared/DTOs/Movies/PersonCreditDto.cs`. If `PersonId` is missing, add it and update the API's projection in `MovieService.GetMovieByIdAsync` to populate it.

- [ ] **Step 3: Commit**

```bash
git add RetroVHS.Client/Components/Pages/Admin/MovieForm.razor
git commit -m "feat(admin): add admin movie create/edit form with person search and genre selection"
```

---

## Task 17: Final Verification

- [ ] **Step 1: Build both projects**

```bash
cd RetroVHS.Api && dotnet build
cd ../RetroVHS.Client && dotnet build
```

Expected: 0 errors in both projects.

- [ ] **Step 2: Verify the ApplicationDbContext has all required DbSets**

Check `RetroVHS.Api/Data/ApplicationDbContext.cs` contains:
- `DbSet<Genre> Genres`
- `DbSet<Person> Persons`
- `DbSet<ProductionCompany> ProductionCompanies`

If any are missing, add them.

- [ ] **Step 3: Smoke-check routing**

Start both API and Client. Verify:
- `/movies` loads the movie list
- `/admin` redirects non-admins to `/`
- Admin user sees "Admin" link in header dropdown
- `/my-orders` shows status badges

- [ ] **Step 4: Final commit**

```bash
git add -A
git commit -m "feat: complete Blazor frontend — user actions, movie list, admin UI"
```
