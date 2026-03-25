# RetroVHS Blazor Frontend Completion — Design Spec
**Date:** 2026-03-25
**Branch:** feature/blazor-ui
**Scope:** RetroVHS.Client project only (no API changes)

---

## Overview

Complete the Blazor frontend for RetroVHS. All API endpoints are already implemented. This spec covers:
1. User feature additions to existing pages
2. A new shared movie list page
3. A new admin service + full admin UI section

---

## 1. User Features

### 1.1 MyOrders.razor — Rental Status Actions

Each rental row displays its current status (`Active`, `Completed`, `Cancelled`).

For rentals with status `Active`, two action buttons appear:
- **Bekräfta leverans** → calls `PUT /api/rentals/{id}/complete` → sets status to `Completed`
- **Avbryt beställning** → calls `PUT /api/rentals/{id}/cancel` → sets status to `Cancelled`

Both actions show an **inline confirmation section** (not a browser alert, not a modal) directly below the row before executing. The confirmation section has "Ja, bekräfta" and "Avbryt" buttons.

API calls are made via `IUserClient` (existing service). After a successful action, the rental status updates in-place without a full page reload.

### 1.2 MovieDetails.razor — Review Display

The review list currently shows `review.UserDisplayName`. Update so that:
- If `UserDisplayName` is null or empty → display **"Anonym"**
- Otherwise display the value as-is

This is a UI-only change unless the API needs to be confirmed to return nickname (to be verified during implementation).

---

## 2. Shared Movie List — `/movies`

### 2.1 Page

New page `Components/Pages/Movies.razor` at route `/movies`.

**Display:** Simple list view (not MovieCards). Each row shows:
- Title
- Release year
- Average rating (stars + numeric value, or "Inga betyg" if no ratings)
- Link to `/movie/{id}` (the existing detail page)
- For authenticated admins only: "Redigera"-link to `/admin/movies/{id}/edit`

**Filtering and sorting (combinable):**
- **Genre filter** — dropdown with all genres extracted client-side from movie data. Filters the list (Where).
- **Sort** — either by **Betyg** (high→low) or **Titel** (A→Z / Z→A). Applied on top of the genre filter (OrderBy after Where).

Genre filter and sort can be used together.

Data source: existing `IMovieClient.GetAllMoviesAsync()`. No new API calls needed.

### 2.2 Home.razor — "Se alla filmer" Link

A "Se alla filmer →" link is added at the top of the main content area on Home.razor (not in the header). It navigates to `/movies`.

---

## 3. Admin Service

New files:
- `RetroVHS.Client/Services/IAdminClient.cs`
- `RetroVHS.Client/Services/AdminClient.cs`

Registered in `Program.cs` as `builder.Services.AddScoped<IAdminClient, AdminClient>()`.

### Methods

| Method | HTTP | Endpoint |
|--------|------|----------|
| `GetDashboardStatsAsync()` | GET | `/api/admin/stats` |
| `GetAllUsersAsync()` | GET | `/api/admin/users` |
| `GetUserByIdAsync(int id)` | GET | `/api/admin/users/{id}` |
| `DeleteUserAsync(int id)` | DELETE | `/api/admin/users/{id}` |
| `BlockUserAsync(int id)` | PUT | `/api/admin/users/{id}/block` |
| `UnblockUserAsync(int id)` | PUT | `/api/admin/users/{id}/unblock` |
| `SetNicknameNullAsync(int id)` | PUT | `/api/admin/users/{id}/nickname` (body: `{ "nickname": null }`) |
| `GetUserReviewsAsync(int id)` | GET | `/api/admin/users/{id}/reviews` |
| `RemoveReviewCommentAsync(int reviewId)` | DELETE | `/api/admin/reviews/{id}/comment` |
| `DeleteReviewAsync(int reviewId)` | DELETE | `/api/admin/reviews/{id}` |
| `GetUserRentalsAsync(int id)` | GET | `/api/admin/users/{id}/rentals` |
| `CancelRentalAsync(int rentalId)` | PUT | `/api/admin/rentals/{id}/cancel` |
| `GetAllMoviesAsync()` | GET | `/api/admin/movies` |
| `GetMovieByIdAsync(int id)` | GET | `/api/admin/movies/{id}` |
| `CreateMovieAsync(CreateMovieDto dto)` | POST | `/api/admin/movies` |
| `UpdateMovieAsync(int id, UpdateMovieDto dto)` | PUT | `/api/admin/movies/{id}` |
| `DeleteMovieAsync(int id)` | DELETE | `/api/admin/movies/{id}` |

---

## 4. Admin Pages

All admin pages use `@attribute [Authorize(Roles = "Admin")]`. Unauthenticated or non-admin users are redirected to `/`.

All destructive/irreversible actions use **inline confirmation sections** (no browser alerts, no modals) — a small confirmation area appears below the triggering element with "Ja" and "Avbryt" buttons. This applies consistently for both user-facing and admin actions.

### 4.1 `/admin` — Dashboard

Displays stat cards from `AdminDashboardDto`:
- Totalt antal användare
- Blockerade användare
- Antal filmer i katalogen
- Aktiva uthyrningar
- Totala uthyrningar
- Totala recensioner

Quick-navigation links to `/admin/users` and `/admin/movies`.

### 4.2 `/admin/users` — User List

Table of all users showing: name, email, nickname, blocked status.
Per-row action links: **Profil** (`/admin/users/{id}`), **Recensioner** (`/admin/users/{id}/reviews`), **Beställningar** (`/admin/users/{id}/orders`).
Per-row delete button with inline confirmation.

### 4.3 `/admin/users/{id}` — User Profile

Displays user info (name, email, nickname, blocked status).
Actions (each with inline confirmation):
- **Sätt nickname till null** — calls `SetNicknameNullAsync(id)`
- **Blockera / Avblockera** — calls `BlockUserAsync` / `UnblockUserAsync`
- **Radera användare** — calls `DeleteUserAsync(id)`, then navigates back to `/admin/users`

Back link to `/admin/users`.

### 4.4 `/admin/users/{id}/reviews` — User Reviews

Lists all reviews for the user. Each review shows: movie title (linked to `/movie/{movieId}`), rating, comment, date.
Per-review actions (each with inline confirmation):
- **Ta bort kommentar** — calls `RemoveReviewCommentAsync(reviewId)` (keeps rating)
- **Radera recension** — calls `DeleteReviewAsync(reviewId)`

Back link to `/admin/users/{id}`.

### 4.5 `/admin/users/{id}/orders` — User Orders

Lists all rentals for the user. Each row shows: movie title, price, status, rental date.
For `Active` rentals: **Avbryt beställning** button with inline confirmation → calls `CancelRentalAsync(rentalId)`.

Back link to `/admin/users/{id}`.

### 4.6 `/admin/movies` — Admin Movie List

Same list layout as `/movies` (title, year, average rating) but with additional per-row buttons:
- **Redigera** → `/admin/movies/{id}/edit`
- **Radera** → calls `DeleteMovieAsync(id)` with inline confirmation

Button at top: **+ Lägg till film** → navigates to `/admin/movies/new`.

### 4.7 `/admin/movies/new` and `/admin/movies/{id}/edit` — Movie Form

Single component `MovieForm.razor` used by both routes (new = no pre-filled data, edit = pre-filled from `GetMovieByIdAsync`).

**Fields:**
| Field | Input type |
|-------|-----------|
| Titel | Text |
| Utgivningsår | Number |
| Längd (minuter) | Number |
| Synopsis | Textarea |
| Poster-URL | Text |
| Trailer-URL | Text |
| Hyrespris (kr) | Decimal number |
| Tillgänglighetsstatus | Select (enum `MovieAvailabilityStatus`) |
| Utvald (IsFeatured) | Checkbox |
| Genres | Dynamic tag list — type a genre name, press Add, removable chips |
| Regissörer | Dynamic list of name fields (add/remove rows) |
| Skådespelare | Dynamic list of rows: name + rollnamn (character name), add/remove |
| Produktionsbolag | Text |

On submit: calls `CreateMovieAsync` or `UpdateMovieAsync`. On success: navigates to `/admin/movies`. Validation errors shown inline.

---

## 5. Header Updates

In `Header.razor`, the authenticated user dropdown menu gets a new item visible only when the user has the `Admin` role:

```
@if (authContext.User.IsInRole("Admin"))
{
    <a class="user-menu-item" href="/admin">Admin</a>
}
```

Placed above the logout button, separated by a divider.

---

## 6. Files to Create / Modify

### New files
- `Services/IAdminClient.cs`
- `Services/AdminClient.cs`
- `Components/Pages/Movies.razor`
- `Components/Pages/Admin/Dashboard.razor`
- `Components/Pages/Admin/Users.razor`
- `Components/Pages/Admin/UserProfile.razor`
- `Components/Pages/Admin/UserReviews.razor`
- `Components/Pages/Admin/UserOrders.razor`
- `Components/Pages/Admin/MovieList.razor`
- `Components/Pages/Admin/MovieForm.razor`  _(shared component for new + edit)_

### Modified files
- `Program.cs` — register `IAdminClient`
- `Components/Pages/Home.razor` — add "Se alla filmer" link, remove old TODO comment
- `Components/Pages/MyOrders.razor` — add status display + confirm/cancel actions
- `Components/Pages/MovieDetails.razor` — show "Anonym" for null UserDisplayName
- `Components/Layout/Header.razor` — add Admin link for admin role

---

## 7. Design Conventions

All new Blazor UI follows the existing RetroVHS design:
- Dark theme background
- Red accent color `#e94560` for primary actions
- Existing CSS classes reused where possible (`section-title`, `btn-submit`, `btn-remove`, `cart-item`, `loading-spinner`, etc.)
- The `frontend-design` skill is invoked for all Blazor file creation/modification
- Inline confirmation pattern: a small `<div class="confirm-inline">` appears below the action with two buttons, no modals
