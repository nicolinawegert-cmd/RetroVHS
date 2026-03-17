# RetroVHS - Grupparbete ASP.NET Core + Blazor

En fullstack-applikation för att sköta VHS-filmuthyrning med authentication, filmkatalog och favoriter.

## 🎬 Projektbeskrivning

RetroVHS är en webbapplikation där användare kan:

- Registrera sig och logga in (JWT-autentisering)
- Söka och filtra filmer
- Markera filmer som favoriter
- Admin kan lägga till/redigera filmer och hantera katalogen

## 🏗️ Arkitektur

```
RetroVHS.Api/               Backend WebAPI
├── Controllers/            API-endpoints
├── Services/               Business logic
├── Repositories/           Data access
├── Models/                 Entity models
├── Auth/                   JWT & security
└── Data/                   Database

RetroVHS.Client/            Frontend Blazor
├── Pages/                  Razor-sidor
├── Components/             Reusable components
├── Services/               Client services
└── Providers/              Auth state

RetroVHS.Shared/            Delade kontrakt
├── DTOs/                   Request/Response models
└── Enums/                  Shared enums

RetroVHS.Tests/             Unit & integration tests
├── Services/               Service tests
├── Controllers/            Controller tests
└── Helpers/                Test utilities
```

## 📋 Arbetsfördelning

Se [TEAM_ALLOCATION.md](TEAM_ALLOCATION.md) för detaljerad arbetsfördelning mellan gruppmedlemmar.

**Övergripande:**

- **Person 1-2**: API-backend (Auth, Movies/Genres, Database)
- **Person 3-4**: Blazor-frontend (Auth pages, UI Components)
- **Person 5-6**: Shared/Test (DTOs, Unit Tests, Docs)

## 🚀 Getting Started

### Förutsättningar

- .NET 7 eller senare
- Visual Studio 2022 eller VS Code
- SQL Server LocalDB eller SQLite

### Installera och köra

```bash
# Klona och navigera till rot-mappen
cd RetroVHS

# Restaurera dependencies
dotnet restore

# Skapa databas (migrations)
cd RetroVHS.Api
dotnet ef database update

# Starta API (port 5000)
dotnet run --project RetroVHS.Api

# I nytt terminal-fönster, starta Client
dotnet run --project RetroVHS.Client
```

Öppna `https://localhost:5001` i webbläsaren.

## 📁 Mappar och ansvar

Se [TEAM_ALLOCATION.md](TEAM_ALLOCATION.md) för detaljerad uppdelning.

## ✅ Krav

Projektet måste uppfylla:

- [ ] RESTful API med proper HTTP-metoder
- [ ] JWT-autentisering
- [ ] Database med Entity Framework
- [ ] Minst 5 Blazor-sidor
- [ ] Minst 3 egna komponenter
- [ ] Minst 10 enhetstester
- [ ] Clean Code & SOLID-principer
- [ ] Swagger-dokumentation (API)
- [ ] Meningsfulla Git-commits

## 🔐 Git-workflow

```bash
# Skapa feature-branch
git checkout -b feature/my-feature

# Gör ändringar
git add .
git commit -m "Add feature X"

# Push och skapa Pull Request
git push origin feature/my-feature
```

Se [TEAM_ALLOCATION.md](TEAM_ALLOCATION.md) för rekommenderade branch-namn per person.

## 📖 Dokumentation

- **API-dokumentation**: `https://localhost:5000/swagger` (Swagger UI)
- **Arkitektur**: Se `RetroVHS.Api/Controllers` → `Services` → `Repositories` → `Database`
- **DTO-kontrakt**: Se `RetroVHS.Shared/DTOs/`

## 🧪 Tester

```bash
dotnet test RetroVHS.Tests
```

Målsättning: **Minst 10 unit tests** + manuell integration-test av frontend↔API

## 📝 Demo-Konton

```
Admin:
Email: admin@retrovhs.com
Lösenord: Admin123!

Vanlig användare:
Email: user@retrovhs.com
Lösenord: User123!
```

## 🎯 Redovisning: 27 mars 2026

**Format:** 15-20 min presentation + 5-10 min frågor

**Innehål:**

1. **Demo** (5-7 min) - Visa appen fungerar
2. **Teknik** (5-7 min) - Förklara arkitektur och intressanta lösningar
3. **Reflektion** (3-5 min) - Vad gick bra? Utmaningar?


