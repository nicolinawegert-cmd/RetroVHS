# RetroVHS - Arbetsfördelning (Team allocation: 6 personer)

Projektstruktur baserad på Clean Architecture med tydlig ansvarsfördelning per person.

---

## 📋 ÖVERSIKT

```
API-teamet (2 personer)           Client-teamet (2 personer)    Shared + Tests (2 personer)
├─ Person 1: Auth & JWT           ├─ Person 3: Auth & Pages     ├─ Person 5: DTOs & Shared
├─ Person 2: Movies/Genres        └─ Person 4: UI & Components  └─ Person 6: Tests & README
```

---

## 👤 PERSON 1: API - Authentication & Security

**Mappar:**

- `RetroVHS.Api/Auth/`
- `RetroVHS.Api/Config/`
- `RetroVHS.Api/Controllers/AuthController.cs`

**Ansvar:**

- [ ] AuthController (login, register, refresh token)
- [ ] AuthService (business logic för auth)
- [ ] TokenService (JWT-token generation)
- [ ] PasswordHasher (password hashing & validation)
- [ ] JwtSettings config
- [ ] User model & UserRepository
- [ ] Skyddade endpoints (Authorize-attributes)
- [ ] Roll-baserad åtkomst (Admin-roll)

**Samarbete:**

- Diskutera User-modellen med Person 2
- Se till att LoginRequestDto och LoginResponseDto från Shared fungerar
- Berätta för Person 3 hur login ska fungera i frontend

**Git-brancher:**

- `feature/auth-setup`
- `feature/jwt-implementation`
- `feature/user-registration`

---

## 👤 PERSON 2: API - Movies, Genres & Data

**Mappar:**

- `RetroVHS.Api/Models/` (Movie, Genre, Favorite)
- `RetroVHS.Api/Repositories/` (MovieRepository, GenreRepository, FavoriteRepository, UserRepository)
- `RetroVHS.Api/Services/` (MovieService, GenreService, FavoriteService)
- `RetroVHS.Api/Controllers/` (MoviesController, GenresController, FavoritesController)
- `RetroVHS.Api/Data/` (AppDbContext, SeedData)

**Ansvar:**

- [ ] Movie, Genre, Favorite models
- [ ] AppDbContext (Entity Framework migrations)
- [ ] Repositories (MovieRepository, GenreRepository, etc.)
- [ ] Services (CRUD-logik)
- [ ] Controllers (endpoints)
- [ ] SeedData (demo-data)
- [ ] Database setup

**Samarbete:**

- Diskutera User-relation med Person 1
- DTOs för Movies, Genres, Favorites ska komma från Shared (Person 5)
- Berätta för Person 4 vilka endpoints som finns

**Git-brancher:**

- `feature/movie-crud`
- `feature/genre-management`
- `feature/favorites`
- `feature/database-setup`

---

## 👤 PERSON 3: Client - Authentication & Pages

**Mappar:**

- `RetroVHS.Client/Pages/` (Login, Register, Protected pages)
- `RetroVHS.Client/Providers/` (CustomAuthStateProvider)
- Viss del av `Services/` (AuthService i client)

**Ansvar:**

- [ ] Login.razor (formulär + validering)
- [ ] Register.razor (ny användare)
- [ ] Home.razor (startsida)
- [ ] Admin/Dashboard.razor (admin-sida, [Authorize] attribute)
- [ ] CustomAuthStateProvider (JWT-hantering i client)
- [ ] AuthService i client (logout, token-hantering)
- [ ] Routing setup (navmenu, protected routes)
- [ ] AuthorizeView för roll-baserad visning

**Samarbete:**

- Fråga Person 1 om auth-flödet (hur login fungerar)
- Koordinera med Person 4 om sidnavigation
- Använd LoginRequestDto och LoginResponseDto från Shared

**Git-brancher:**

- `feature/auth-pages`
- `feature/custom-auth-provider`
- `feature/routing-setup`

---

## 👤 PERSON 4: Client - UI & Components

**Mappar:**

- `RetroVHS.Client/Pages/` (Movies, MovieDetails, ManageMovies)
- `RetroVHS.Client/Components/` (MovieCard, MovieForm, MovieFilter, Header, etc.)
- `RetroVHS.Client/Services/` (ApiService, MovieService i client)

**Ansvar:**

- [ ] Movies.razor (filmlistning)
- [ ] MovieDetails.razor (detaljer för en film)
- [ ] ManageMovies.razor (admin-sida för att lägga till/redigera filmer)
- [ ] MovieCard.razor component (återanvändbar)
- [ ] MovieForm.razor component (för create/update)
- [ ] MovieFilter.razor component (sökning och filtrering)
- [ ] RetroHeader.razor (navigation header)
- [ ] ApiService (HTTP-kommunikation)
- [ ] MovieService i client (API-anrop för filmer)
- [ ] Styling & layout

**Samarbete:**

- Fråga Person 2 om endpoints för filmer
- Koordinera med Person 3 om routing
- Person 5 ger MovieResponseDto och CreateMovieDto

**Git-brancher:**

- `feature/movie-pages`
- `feature/movie-components`
- `feature/api-integration`

---

## 👤 PERSON 5: Shared - DTOs & API-kontrakt

**Mappar:**

- `RetroVHS.Shared/DTOs/` (Auth, Movies, Genres, Favorites)
- `RetroVHS.Shared/Enums/` (UserRole, MovieStatus)

**Ansvar:**

- [ ] LoginRequestDto, LoginResponseDto, RegisterRequestDto
- [ ] CreateMovieDto, UpdateMovieDto, MovieResponseDto
- [ ] GenreResponseDto, CreateGenreDto
- [ ] FavoriteDto
- [ ] UserRole enum (User, Admin)
- [ ] MovieStatus enum (Available, Rented, etc.)
- [ ] Se till att alla DTO:er är dokumenterade
- [ ] Validering-attributes på DTOs (DataAnnotations)

**Samarbete:**

- Diskutera med Person 1 & 2 (API) vilka properties DTOs behöver
- Diskutera med Person 3 & 4 (Client) hur DTOs används
- Var först med DTO-designen - andra teamet väntar på detta!

**Git-brancher:**

- `feature/shared-dtos`
- `feature/enums`

**💡 Tips:** Börja här! API och Client kan inte flytta långt utan dessa DTOs.

---

## 👤 PERSON 6: Tests, README & Integration

**Mappar:**

- `RetroVHS.Tests/` (alla testfiler)
- ROOT: README.md, .env, launch-instruktioner

**Ansvar:**

- [ ] MovieServiceTests (minst 3-4 tester)
- [ ] AuthServiceTests (login, register, token)
- [ ] GenreServiceTests
- [ ] MoviesControllerTests
- [ ] SeedData (demo-konton och film-data för test)
- [ ] README.md (körninstruktioner, arkitektur-diagram)
- [ ] TEAM_ALLOCATION.md (denna fil)
- [ ] Se till att projeto kör från start till slut
- [ ] Dokumentera hur man skapar databasen
- [ ] Testa frontend-backend integration manuellt

**Samarbete:**

- Få insyn i vad API och Client gör
- Skapa testdata som matchar den verkliga kärnor
- Hjälp API- och Client-teamet med integrationen när de fastnar

**Git-brancher:**

- `feature/unit-tests`
- `feature/documentation`
- `feature/test-data`

**Demo-konton:**

```
Admin:
- Email: admin@retrovhs.com
- Password: Admin123!

Regular User:
- Email: user@retrovhs.com
- Password: User123!
```

---

## 🔗 KRITISKA SAMARBETEN

### API ↔ Shared

- Person 1 & 2 arbetar med Person 5 på DTO-design
- DTOs ska vara klara innan API implementeras fullständigt

### Client ↔ Shared

- Person 3 & 4 använder DTOs från Person 5
- Endpoints måste matcha det API:et exponerar

### Tests ↔ Alla

- Person 6 behöver tillgång till alla services
- Skapar testdata som passar hela flödet

---

## 📅 TIDSLINJE (Vecka 8-13)

### Vecka 8: UPPSTART

- [ ] Alla läser denna fil och förstår sitt ansvar
- [ ] Person 5: Designa DTOs (PRIORITET!)
- [ ] Person 1 & 2: Sätt upp project-struktur
- [ ] Person 3 & 4: Förbered Blazor-projekt
- [ ] Alla: Initiera Git, brancher

### Vecka 9: GRUNDLÄGGANDE

- [ ] Person 1: Auth-tjänster
- [ ] Person 2: Database & Movie CRUD
- [ ] Person 3: Login/Register-sidor
- [ ] Person 4: MovieCard & ApiService
- [ ] Person 6: Börja tester och README

### Vecka 10-11: INTEGRERING

- [ ] API och Client jobbar tillsammans
- [ ] Person 6: Integrationstest

### Vecka 12: POLERING

- [ ] Bugfixar
- [ ] Styling
- [ ] Sista tester

### Vecka 13: REDOVISNING

- [ ] Finalisera presentation
- [ ] Testa allt funkar
- [ ] Deployment/demo-data

---

## ✅ CHECKLISTA PER PERSON

### Person 1 (Auth API)

- [ ] AuthController med login/register/refresh endpoints
- [ ] JWT-tokens genereras korrekt
- [ ] Passwords hashas säkert
- [ ] User-modellen är klar
- [ ] Minst 2 enhetstester för AuthService

### Person 2 (Movies API)

- [ ] Movie, Genre, Favorite models
- [ ] Database migrations
- [ ] Repositories implementerade
- [ ] Services implementerade
- [ ] Controllers med alla endpoints
- [ ] SeedData med 5-10 demo-filmer
- [ ] Minst 2 enhetstester per service

### Person 3 (Auth Client)

- [ ] Login.razor med form
- [ ] Register.razor med form
- [ ] CustomAuthStateProvider
- [ ] AuthService (client-version)
- [ ] Routing setup
- [ ] [Authorize] på protected sidor

### Person 4 (Movies Client)

- [ ] Movies.razor
- [ ] MovieDetails.razor
- [ ] ManageMovies.razor
- [ ] Minst 3 egna components
- [ ] ApiService
- [ ] MovieService (client-version)
- [ ] Styling & layout

### Person 5 (Shared)

- [ ] Alla DTOs designade och dokumenterade
- [ ] Alla Enums
- [ ] Validering-attributes på DTOs
- [ ] Eventuell styling-gemensam CSS

### Person 6 (Tests & Docs)

- [ ] Minst 10 enhetstester totalt
- [ ] SeedData klart
- [ ] README.md klart
- [ ] Testat hela flödet end-to-end
- [ ] Dokumenterat ansvarsfördelning

---

## 🚀 NÄSTA STEG

1. **Dela upp denna fil** i gruppen
2. **Person 5 börjar först** - designa DTOs
3. **Varje person skapar sin feature-branch**
4. **Möte varje dag** för status-uppdateringar
5. **Person 6 är "scrum-master"** - håller ordning och integrerar allt

**Lycka till! 🎬**
