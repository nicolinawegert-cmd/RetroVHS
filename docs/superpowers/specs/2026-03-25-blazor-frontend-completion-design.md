# RetroVHS Blazor Frontend Completion — Design Spec
**Date:** 2026-03-25
**Branch:** feature/blazor-ui
**Scope:** RetroVHS.Client (primary) + minor read-only additions to RetroVHS.Api

---

## Overview

Complete the Blazor frontend for RetroVHS. Most API endpoints are already implemented. This spec covers:
1. User feature additions to existing pages
2. A new shared movie list page
3. Three new read-only API endpoints (genres, persons, production companies)
4. A new admin service + full admin UI section

---

## 1. User Features

### 1.1 MyOrders.razor — Rental Status Actions

Each rental row displays its current status badge:
- `Active` → "Aktiv" (green/accent badge)
- `Completed` → "Levererad" (muted badge)
- `Cancelled` → "Avbruten" (muted badge)
- `Expired` → "Utgången" (muted badge) — display only, no actions

For `Active` rentals only, two action buttons appear:
- **Bekräfta leverans** → calls `PUT /api/rentals/{id}/complete` → status becomes `Completed`
- **Avbryt beställning** → calls `PUT /api/rentals/{id}/cancel` → status becomes `Cancelled`

Both actions show an **inline confirmation section** (not a browser alert, not a modal) directly below the row before executing. The confirmation section contains "Ja, bekräfta" and "Avbryt" buttons.

After a successful action, the rental row updates in-place (status badge changes, action buttons disappear) without a full page reload.

**`IUserClient` extension** — two new methods added:
```csharp
Task<bool> CompleteRentalAsync(int rentalId);  // PUT /api/rentals/{id}/complete
Task<bool> CancelRentalAsync(int rentalId);    // PUT /api/rentals/{id}/cancel
```
These call `RentalsController` endpoints, not a users endpoint.

### 1.2 MovieDetails.razor — Review Display

The review list currently uses `review.UserDisplayName` (which the API resolves as nickname if `UseNickname=true`, else full name). Update the UI so that if the resolved value is null or empty → display **"Anonym"**.

---

## 2. Shared Movie List — `/movies`

### 2.1 Page

New page `Components/Pages/Movies.razor` at route `/movies`.

**Display:** Simple list view (not MovieCards). Each row shows:
- Title
- Release year
- Average rating: filled stars + numeric (e.g. ★★★☆☆ 3.2), or "Inga betyg" if `RatingCount == 0`
- Clickable row / title link → `/movie/{id}`
- For authenticated admins only: "Redigera"-link per row → `/admin/movies/{id}/edit`

**Filtering and sorting (combinable):**
- **Genre filter** — dropdown of unique genre names extracted client-side from `MovieListDto.Genres`. Applies a LINQ `Where` (client-side) filtering movies whose `Genres` list contains the selected name. Default: "Alla genres".
- **Sort** — radio/select with three options:
  - Titel A→Z (default)
  - Betyg (högt→lågt)
  - Titel Z→A
- Genre filter and sort apply together: filter first, then sort.

**Performance note:** All movies are loaded once via `IMovieClient.GetAllMoviesAsync()` and filtering/sorting are applied entirely client-side with LINQ. This is intentional given current dataset size.

### 2.2 Home.razor — "Se alla filmer" Link

A "Se alla filmer →" link is added at the top of the main content area in `Home.razor` (below the `<PageTitle>` but above any movie sections, not in the header). Remove the existing TODO comment about the all-movies section.

---

## 3. New API Endpoints (RetroVHS.Api)

Three new read-only endpoints are added to support the admin movie form. These require no authentication.

### 3.1 GET /api/genres
Returns all genres: `List<GenreDto>` where `GenreDto` has `int Id` and `string Name`.
Added to `MoviesController` (or a new `GenresController`).

### 3.2 GET /api/persons?search={term}
Returns persons matching search term (by name): `List<PersonDto>` where `PersonDto` has `int Id` and `string FullName`.
Optional `search` query parameter. Returns all if omitted, filtered by name if provided.
Added to a new `PersonsController`.

### 3.3 GET /api/production-companies
Returns all production companies: `List<ProductionCompanyDto>` where `ProductionCompanyDto` has `int Id` and `string Name`.
Added to a new `ProductionCompaniesController`.

**Corresponding DTOs** added to `RetroVHS.Shared`:
- `GenreDto` — `int Id`, `string Name`
- `PersonDto` — `int Id`, `string FullName`
- `ProductionCompanyDto` — `int Id`, `string Name`

These DTOs and endpoints are also accessible via `IAdminClient` (see Section 4).

---

## 4. Admin Service

New files:
- `RetroVHS.Client/Services/IAdminClient.cs`
- `RetroVHS.Client/Services/AdminClient.cs`

Registered in `Program.cs`:
```csharp
builder.Services.AddScoped<IAdminClient, AdminClient>();
```

### Full Interface

```csharp
public interface IAdminClient
{
    // Dashboard
    Task<AdminDashboardDto?> GetDashboardStatsAsync();

    // Users
    Task<List<UserDto>> GetAllUsersAsync();
    Task<UserDto?> GetUserByIdAsync(int id);
    Task<bool> DeleteUserAsync(int id);          // DELETE /api/admin/users/{id}
    Task<bool> BlockUserAsync(int id);           // PUT /api/admin/users/{id}/block
    Task<bool> UnblockUserAsync(int id);         // PUT /api/admin/users/{id}/unblock
    Task<bool> SetNicknameNullAsync(int id);     // PUT /api/admin/users/{id}/nickname  body: { "nickname": null }

    // Reviews (admin)
    Task<List<ReviewDto>> GetUserReviewsAsync(int userId);           // GET /api/admin/users/{id}/reviews
    Task<bool> RemoveReviewCommentAsync(int reviewId);               // DELETE /api/admin/reviews/{id}/comment
    Task<bool> DeleteReviewAsync(int reviewId);                      // DELETE /api/admin/reviews/{id}

    // Rentals (admin)
    Task<List<RentalDto>> GetUserRentalsAsync(int userId);           // GET /api/admin/users/{id}/rentals
    Task<bool> CancelRentalAsync(int rentalId);                      // PUT /api/admin/rentals/{id}/cancel

    // Movies (admin)
    Task<List<MovieListDto>> GetAllMoviesAsync();                    // GET /api/admin/movies
    Task<MovieDetailsDto?> GetMovieByIdAsync(int id);                // GET /api/admin/movies/{id}
    Task<MovieDetailsDto?> CreateMovieAsync(CreateMovieDto dto);     // POST /api/admin/movies
    Task<MovieDetailsDto?> UpdateMovieAsync(int id, UpdateMovieDto dto); // PUT /api/admin/movies/{id}
    Task<bool> DeleteMovieAsync(int id);                             // DELETE /api/admin/movies/{id}

    // Reference data (new read-only endpoints)
    Task<List<GenreDto>> GetGenresAsync();                           // GET /api/genres
    Task<List<PersonDto>> GetPersonsAsync(string? search = null);    // GET /api/persons?search={term}
    Task<List<ProductionCompanyDto>> GetProductionCompaniesAsync();  // GET /api/production-companies
}
```

---

## 5. Admin Pages

All admin pages use `@attribute [Authorize(Roles = "Admin")]`. Non-admin or unauthenticated users are redirected to `/`.

All destructive/irreversible actions use **inline confirmation sections** — a confirmation area appears directly below the triggering element with "Ja" and "Avbryt" buttons. No browser `alert()`, no modals. This applies consistently for both user-facing and admin actions throughout the app.

### 5.1 `/admin` — Dashboard

Displays stat cards from `AdminDashboardDto`:
- Totalt antal användare
- Blockerade användare
- Antal filmer i katalogen
- Aktiva uthyrningar
- Totala uthyrningar
- Totala recensioner

Quick-navigation links to `/admin/users` and `/admin/movies`.

### 5.2 `/admin/users` — User List

Table of all users: name, email, nickname (or "—"), blocked status badge.
Per-row links: **Profil** (`/admin/users/{id}`), **Recensioner** (`/admin/users/{id}/reviews`), **Beställningar** (`/admin/users/{id}/orders`).
Per-row **Radera** button with inline confirmation → calls `DeleteUserAsync(id)`, refreshes list on success.

### 5.3 `/admin/users/{id}` — User Profile

Displays: name, email, nickname, blocked status, registration date (if available).
Actions (each with inline confirmation):
- **Sätt nickname till null** — calls `SetNicknameNullAsync(id)`, updates display
- **Blockera** / **Avblockera** — calls `BlockUserAsync` / `UnblockUserAsync`, toggles based on current status
- **Radera användare** — calls `DeleteUserAsync(id)`, navigates to `/admin/users` on success

Navigation: back link to `/admin/users`, links to this user's Recensioner and Beställningar.

### 5.4 `/admin/users/{id}/reviews` — User Reviews

Lists all reviews for the user. Each review shows: movie title (linked to `/movie/{movieId}`), star rating, comment (or "Ingen kommentar"), date.

Per-review actions (each with inline confirmation):
- **Ta bort kommentar** — calls `RemoveReviewCommentAsync(reviewId)`, updates row (comment disappears, button disabled)
- **Radera recension** — calls `DeleteReviewAsync(reviewId)`, removes row

Back link to `/admin/users/{id}`.

### 5.5 `/admin/users/{id}/orders` — User Orders

Lists all rentals for the user. Each row: movie title, price paid, status badge, rental date.
For `Active` rentals: **Avbryt beställning** button with inline confirmation → calls `CancelRentalAsync(rentalId)` (admin version via `IAdminClient`), updates status badge in-place.

Back link to `/admin/users/{id}`.

### 5.6 `/admin/movies` — Admin Movie List

Same list layout as `/movies` (title, year, avg rating) with additional per-row buttons:
- **Redigera** → navigates to `/admin/movies/{id}/edit`
- **Radera** → inline confirmation → calls `DeleteMovieAsync(id)`, removes row

Button at top of page: **+ Lägg till film** → navigates to `/admin/movies/new`.

### 5.7 `/admin/movies/new` and `/admin/movies/{id}/edit` — Movie Form

Route `/admin/movies/new` renders `MovieForm.razor` with empty state.
Route `/admin/movies/{id}/edit` renders `MovieForm.razor` pre-filled via `GetMovieByIdAsync(id)`.

`MovieForm.razor` is located in `Components/Pages/Admin/` (admin-only component).

**Fields:**

| Field | Input type | Notes |
|-------|-----------|-------|
| Titel | Text (required) | |
| Utgivningsår | Number (required, 1910–2100) | |
| Längd (minuter) | Number (required, 1–600) | |
| Synopsis | Textarea (required, max 1000 chars) | |
| Poster-URL | Text (optional) | |
| Trailer-URL | Text (optional) | |
| Hyrespris (kr) | Decimal (required, 0.01–999.99) | |
| Lagersaldo | Number (0–10000, default 0) | `StockQuantity` |
| Tillgänglighetsstatus | Select (enum `MovieAvailabilityStatus`) | |
| Utvald (IsFeatured) | Checkbox | |
| Språk | Text (optional) | |
| Land | Text (optional) | |
| Produktionsbolag | Searchable dropdown (optional) | Populated from `GetProductionCompaniesAsync()` |
| Genres | Multi-select tag list | Populated from `GetGenresAsync()`. Admin picks from existing genres by name; IDs resolved client-side. |
| Regissörer | Dynamic list of person-search rows | Each row: searchable input → `GetPersonsAsync(term)` autocomplete, selects PersonId |
| Skådespelare | Dynamic list of rows | Each row: person search (PersonId) + character name (text) + display order (number) |

**Genres flow:** Load all genres on form init. Display as a multi-select list or checkbox group with genre names. Selected genres map to their IDs for the DTO.

**Person search flow (directors + cast):** As admin types in a name field, debounced call to `GetPersonsAsync(term)` populates a small dropdown. Selecting a result stores the `PersonId`. The displayed name is the `FullName` from the result.

**On submit:**
- New: calls `CreateMovieAsync(dto)`, navigates to `/admin/movies` on success
- Edit: calls `UpdateMovieAsync(id, dto)`, navigates to `/admin/movies` on success
- Validation errors shown inline below each field

---

## 6. Header Updates

In `Header.razor`, the admin link is added **inside the existing `<Authorized>` block** using a nested `<AuthorizeView Roles="Admin">`:

```razor
<AuthorizeView Roles="Admin" Context="adminCtx">
    <Authorized>
        <div class="user-menu-divider"></div>
        <a class="user-menu-item" href="/admin">
            <!-- shield SVG icon -->
            Admin
        </a>
    </Authorized>
</AuthorizeView>
```

This is placed inside the existing `<AuthorizeView>` / `<Authorized>` dropdown block, above the logout divider. This ensures the link is invisible to non-admin authenticated users.

---

## 7. Files to Create / Modify

### New files — RetroVHS.Api
- `Controllers/GenresController.cs` (or method added to `MoviesController.cs`)
- `Controllers/PersonsController.cs`
- `Controllers/ProductionCompaniesController.cs`

### New DTOs — RetroVHS.Shared
- `DTOs/Genres/GenreDto.cs`
- `DTOs/Persons/PersonDto.cs`
- `DTOs/ProductionCompanies/ProductionCompanyDto.cs`

### New files — RetroVHS.Client
- `Services/IAdminClient.cs`
- `Services/AdminClient.cs`
- `Components/Pages/Movies.razor`
- `Components/Pages/Admin/Dashboard.razor`
- `Components/Pages/Admin/Users.razor`
- `Components/Pages/Admin/UserProfile.razor`
- `Components/Pages/Admin/UserReviews.razor`
- `Components/Pages/Admin/UserOrders.razor`
- `Components/Pages/Admin/MovieList.razor`
- `Components/Pages/Admin/MovieForm.razor`

### Modified files — RetroVHS.Client
- `Program.cs` — register `IAdminClient`
- `Services/IUserClient.cs` — add `CompleteRentalAsync` and `CancelRentalAsync`
- `Services/UserClient.cs` — implement the two new methods
- `Components/Pages/Home.razor` — add "Se alla filmer" link, remove TODO comment
- `Components/Pages/MyOrders.razor` — status badges + confirm/cancel actions
- `Components/Pages/MovieDetails.razor` — "Anonym" fallback for null UserDisplayName
- `Components/Layout/Header.razor` — Admin link for Admin role

---

## 8. Design Conventions

All new and modified Blazor UI follows the existing RetroVHS design system:
- Dark theme background (`#1a1a2e`, `#16213e`)
- Red accent `#e94560` for primary action buttons
- Reuse existing CSS classes: `section-title`, `btn-submit`, `btn-remove`, `cart-item`, `loading-spinner`, `review-error`, `profile-success`, etc.
- The `frontend-design` skill is invoked for every Blazor file creation/modification
- **Inline confirmation pattern:** a `<div>` with a question text + "Ja" + "Avbryt" buttons appears inline below the triggering element, replacing the trigger button area. No browser alerts, no modals.
- Status badges use small `<span>` elements with class variations (e.g., `status-active`, `status-completed`, `status-cancelled`, `status-expired`)
