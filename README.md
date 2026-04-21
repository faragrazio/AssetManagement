<div align="center">

# 🏭 Asset & Maintenance Management API

**API REST enterprise-grade per la gestione di asset aziendali e ordini di manutenzione**

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=for-the-badge&logo=dotnet)](https://dotnet.microsoft.com)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-336791?style=for-the-badge&logo=postgresql&logoColor=white)](https://postgresql.org)
[![Docker](https://img.shields.io/badge/Docker-ready-2496ED?style=for-the-badge&logo=docker&logoColor=white)](https://docker.com)
[![JWT](https://img.shields.io/badge/Auth-JWT-000000?style=for-the-badge&logo=jsonwebtokens)](https://jwt.io)
[![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)](LICENSE)

[Funzionalità](#-funzionalità) • [Architettura](#-architettura) • [Tech Stack](#-tech-stack) • [Avvio rapido](#-avvio-rapido) • [API Reference](#-api-reference) • [Struttura](#-struttura-del-progetto)

</div>

---

## 📋 Panoramica

Questo progetto è un'**API REST completa** per la gestione di asset aziendali (macchinari, attrezzature, veicoli) e dei relativi ordini di manutenzione. Costruita seguendo principi di **Clean Architecture**, **CQRS** e **Domain-Driven Design**, dimostra l'implementazione di pattern architetturali moderni richiesti in contesti enterprise.

### Cosa rende questo progetto interessante

- **Clean Architecture rigorosa** — Domain completamente isolato, zero dipendenze NuGet nel core
- **CQRS con MediatR** — Commands e Queries separati, pipeline di comportamenti automatica
- **Domain Model ricco** — Le regole di business vivono nelle entità, non nei service
- **JWT stateless** — Autenticazione scalabile senza sessioni server-side
- **Containerizzato** — Docker Compose per avvio one-command dell'intera infrastruttura
- **DateTime UTC corretto** — Gestione esplicita dei timezone con PostgreSQL

---

## ✨ Funzionalità

### Asset Management
- ✅ Creazione asset con validazione (nome, numero seriale univoco, categoria, posizione)
- ✅ Aggiornamento dati descrittivi
- ✅ Transizioni di stato controllate: `Active → InMaintenance → Active` o `→ Decommissioned`
- ✅ Filtro per categoria e stato
- ✅ Vincolo di unicità sul numero seriale (livello applicazione + database)

### Maintenance Orders
- ✅ Apertura ordine su un asset (mette automaticamente l'asset `InMaintenance`)
- ✅ Gestione ciclo di vita: `Pending → InProgress → Completed/Cancelled`
- ✅ Note di completamento dal tecnico
- ✅ Ripristino automatico asset ad `Active` al completamento ordine
- ✅ Filtro per stato, asset, tecnico assegnato
- ✅ Identificazione ordini scaduti

### Authentication & Authorization
- ✅ Registrazione utente con validazione password complessa
- ✅ Login con generazione JWT (24h scadenza)
- ✅ Protezione endpoint tramite `[Authorize]`
- ✅ Ruoli: `Admin`, `Technician`, `Viewer`
- ✅ Soft delete utenti (storicizzazione ordini preservata)

---

## 🏗 Architettura

Il progetto segue la **Clean Architecture** di Robert C. Martin con la regola fondamentale: **le dipendenze puntano sempre verso l'interno**.

```
┌─────────────────────────────────────────────────────────┐
│                        API Layer                         │
│           Controllers • Middleware • Program.cs          │
├─────────────────────────────────────────────────────────┤
│                   Infrastructure Layer                   │
│      EF Core • Repositories • JWT • BCrypt • Npgsql     │
├─────────────────────────────────────────────────────────┤
│                   Application Layer                      │
│    Commands • Queries • Handlers • DTOs • Validators     │
├─────────────────────────────────────────────────────────┤
│                     Domain Layer                         │
│         Entities • Enums • Exceptions • Interfaces       │
│              ← ZERO dipendenze esterne →                 │
└─────────────────────────────────────────────────────────┘
```

### Flusso di una Request HTTP

```
HTTP Request
    │
    ▼
[Controller]
    │ crea Command/Query
    ▼
[MediatR]
    │
    ├──► [LoggingBehavior]     ← logga inizio/fine + tempo esecuzione
    │
    ├──► [ValidationBehavior]  ← esegue FluentValidation
    │         │
    │         └── errori? → HTTP 400 (blocca l'Handler)
    │
    ▼
[Handler]
    │ usa Repository (interfaccia)
    ▼
[Repository] ← Infrastructure implementa il contratto Domain
    │
    ▼
[PostgreSQL]
    │
    ▼
Result<T> → Controller → HTTP Response
```

### CQRS Pattern

```
                    ┌─────────────┐
                    │  MediatR    │
                    └──────┬──────┘
                           │
              ┌────────────┴────────────┐
              │                         │
         COMMANDS                    QUERIES
    (modificano lo stato)        (leggono dati)
              │                         │
    ┌─────────▼──────────┐   ┌──────────▼─────────┐
    │ CreateAssetCommand  │   │  GetAllAssetsQuery  │
    │ UpdateAssetCommand  │   │  GetAssetByIdQuery  │
    │ DeleteAssetCommand  │   │  GetAllOrdersQuery  │
    │ CreateOrderCommand  │   │  GetOrdersByAsset   │
    │ UpdateOrderStatus   │   └────────────────────┘
    │ LoginCommand        │
    │ RegisterCommand     │
    └────────────────────┘
```

---

## 🛠 Tech Stack

| Categoria | Tecnologia | Versione | Scopo |
|-----------|-----------|---------|-------|
| **Runtime** | .NET | 10.0 | Framework principale |
| **Web** | ASP.NET Core Web API | 10.0 | Layer HTTP |
| **Database** | PostgreSQL | 16 | Database relazionale |
| **ORM** | Entity Framework Core | 10.0 | Operazioni CRUD e migrations |
| **Micro-ORM** | Dapper | 2.1 | Disponibile per query SQL complesse (EF Core usato di default) |
| **Mediator** | MediatR | 12.4 | CQRS e pipeline behaviors |
| **Validation** | FluentValidation | 12.1 | Validazione request |
| **Auth** | JWT Bearer | 10.0 | Autenticazione stateless |
| **Hashing** | BCrypt.Net-Next | 4.1 | Hashing password sicuro |
| **Docs** | Swashbuckle | 10.1 | Swagger UI |
| **Container** | Docker + Compose | - | Containerizzazione |

---

## 🚀 Avvio Rapido

### Opzione A — Con Docker (consigliata)

Avvia l'intera infrastruttura (API + PostgreSQL) con un solo comando. L'API viene esposta sulla porta **8080**.

```bash
# Clona il repository
git clone https://github.com/faragrazio/AssetManagement.git
cd AssetManagement

# Avvia tutto con un comando
docker compose up -d

# L'API sarà disponibile su http://localhost:8080/swagger
```

### Opzione B — Sviluppo locale

L'API viene avviata sulla porta **5211** (configurazione di default di Kestrel).

#### Prerequisiti
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [PostgreSQL 17](https://www.postgresql.org/download/)
- [dotnet-ef tool](https://learn.microsoft.com/en-us/ef/core/cli/dotnet)

```bash
# Installa dotnet-ef
dotnet tool install --global dotnet-ef

# Clona il repository
git clone https://github.com/faragrazio/AssetManagement.git
cd AssetManagement

# Crea il database e applica le migrations
dotnet ef database update \
  --project src/AssetManagement.Infrastructure \
  --startup-project src/AssetManagement.API

# Avvia l'API
dotnet run --project src/AssetManagement.API
```

#### Configurazione `appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=asset_management;Username=postgres;Password=postgres"
  },
  "Jwt": {
    "Key": "AssetManagement_SuperSecretKey_2026_MinLength32Chars!",
    "Issuer": "AssetManagement.API",
    "Audience": "AssetManagement.Client",
    "ExpiryHours": "24"
  }
}
```

### Verifica installazione

| Modalità | Swagger UI |
|----------|-----------|
| Docker | `http://localhost:8080/swagger` |
| Locale | `http://localhost:5211/swagger` |

---

## 📡 API Reference

### Authentication

#### Registrazione
```http
POST /api/Auth/register
Content-Type: application/json

{
  "firstName": "Mario",
  "lastName": "Rossi",
  "email": "mario@example.com",
  "password": "Password1",
  "role": "Admin"
}
```

**Risposta:** `201 Created` → `{ "id": 1 }`

#### Login
```http
POST /api/Auth/login
Content-Type: application/json

{
  "email": "mario@example.com",
  "password": "Password1"
}
```

**Risposta:** `200 OK`
```json
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "email": "mario@example.com",
  "fullName": "Mario Rossi",
  "role": "Admin",
  "expiresAt": "2026-04-22T09:00:00Z"
}
```

> ⚠️ **Tutti gli endpoint seguenti richiedono:** `Authorization: Bearer {token}`

---

### Assets

#### Lista asset
```http
GET /api/Assets
GET /api/Assets?category=Macchinario
Authorization: Bearer {token}
```

#### Crea asset
```http
POST /api/Assets
Authorization: Bearer {token}
Content-Type: application/json

{
  "name": "Tornio CNC Reparto 3",
  "serialNumber": "SN-001",
  "category": "Macchinario",
  "location": "Reparto 3",
  "purchaseDate": "2024-01-15T00:00:00Z"
}
```

**Risposta:** `201 Created` → `{ "id": 1 }`

#### Aggiorna asset
```http
PUT /api/Assets/1
Authorization: Bearer {token}
Content-Type: application/json

{
  "id": 1,
  "name": "Tornio CNC Reparto 3 - Aggiornato",
  "category": "Macchinario Pesante",
  "location": "Reparto 4"
}
```

**Risposta:** `204 No Content`

---

### Maintenance Orders

#### Crea ordine di manutenzione
```http
POST /api/MaintenanceOrders
Authorization: Bearer {token}
Content-Type: application/json

{
  "assetId": 1,
  "title": "Sostituzione cinghia motore",
  "description": "Cinghia principale usurata, sostituzione urgente",
  "priority": 3,
  "assignedTo": "Tecnico Bianchi",
  "scheduledDate": "2026-04-25T08:00:00Z"
}
```

> Questo mette automaticamente l'asset in stato `InMaintenance`

#### Aggiorna stato ordine
```http
PATCH /api/MaintenanceOrders/1/status
Authorization: Bearer {token}
Content-Type: application/json

{
  "orderId": 1,
  "newStatus": 3,
  "completionNotes": "Cinghia sostituita, macchina testata e funzionante"
}
```

> Completare un ordine riporta automaticamente l'asset ad `Active`

---

### Valori Enum

#### AssetStatus
| Valore | Codice | Descrizione |
|--------|--------|-------------|
| `Active` | 1 | Operativo e disponibile |
| `InMaintenance` | 2 | Temporaneamente fuori servizio |
| `Decommissioned` | 3 | Dismesso definitivamente |

#### OrderStatus
| Valore | Codice | Descrizione |
|--------|--------|-------------|
| `Pending` | 1 | In attesa di presa in carico |
| `InProgress` | 2 | Manutenzione in corso |
| `Completed` | 3 | Completata con successo |
| `Cancelled` | 4 | Annullata |

#### Priority
| Valore | Codice | Descrizione |
|--------|--------|-------------|
| `Low` | 1 | Manutenzione programmata |
| `Medium` | 2 | Da pianificare a breve |
| `High` | 3 | Intervento urgente |
| `Critical` | 4 | Fermo macchina |

---

## 📁 Struttura del Progetto

```
AssetManagement/
├── src/
│   ├── AssetManagement.Domain/          # Zero dipendenze — cuore del sistema
│   │   ├── Entities/                    # Asset, MaintenanceOrder, User
│   │   ├── Enums/                       # AssetStatus, OrderStatus, Priority
│   │   ├── Exceptions/                  # DomainException, NotFoundException
│   │   └── Interfaces/                  # IRepository<T>, IAssetRepository, ...
│   │
│   ├── AssetManagement.Application/     # Logica di business
│   │   ├── Assets/Commands/             # CreateAsset, UpdateAsset, DeleteAsset
│   │   ├── Assets/Queries/              # GetAllAssets, GetAssetById + DTOs
│   │   ├── MaintenanceOrders/           # CreateOrder, UpdateOrderStatus + Queries
│   │   ├── Auth/Commands/               # Login, Register
│   │   └── Common/                      # Behaviors, Interfaces, Result<T>
│   │
│   ├── AssetManagement.Infrastructure/  # Implementazioni concrete
│   │   ├── Persistence/
│   │   │   ├── Configurations/          # EF Core entity mappings
│   │   │   ├── Repositories/            # AssetRepository, OrderRepository, ...
│   │   │   └── AssetManagementDbContext.cs
│   │   └── Identity/                    # JwtService, PasswordHasher
│   │
│   └── AssetManagement.API/             # Entry point
│       ├── Controllers/                 # AssetsController, AuthController, ...
│       ├── Middleware/                  # ExceptionHandlingMiddleware
│       ├── Extensions/                  # ServiceCollectionExtensions
│       └── Program.cs
│
├── Dockerfile                           # Multi-stage build
├── docker-compose.yml                   # PostgreSQL + API
└── AssetManagement.sln
```

---

## 🔄 Transizioni di Stato

### Asset State Machine

```
                    ┌─────────┐
         ───────────►  Active  ◄──────────────────┐
         │          └────┬────┘                    │
         │               │ StartMaintenance()       │
         │               ▼                          │ CompleteMaintenance()
         │         ┌─────────────┐                  │
         │         │InMaintenance│──────────────────┘
         │         └─────────────┘
         │
    (non reversibile)
         │          ┌────────────────┐
         └──────────►  Decommissioned │
                    └────────────────┘
```

### Maintenance Order State Machine

```
┌─────────┐   Start()    ┌────────────┐   Complete()  ┌───────────┐
│ Pending ├─────────────►│ InProgress ├──────────────►│ Completed │
└────┬────┘              └─────┬──────┘               └───────────┘
     │                         │
     │ Cancel()                │ Cancel()
     ▼                         ▼
┌───────────┐          ┌───────────┐
│ Cancelled │          │ Cancelled │
└───────────┘          └───────────┘
```

---

## 🔒 Sicurezza

- **Password hashing** con BCrypt (work factor 12 — ~300ms per hash)
- **JWT** firmato con HMAC-SHA256, chiave minimo 32 caratteri
- **Messaggio generico** al login errato (no user enumeration)
- **Endpoint protetti** con `[Authorize]` — 401 senza token valido
- **Validazione input** su tutti gli endpoint con FluentValidation
- **Vincoli DB** — indici unici su SerialNumber e Email

---

## 💡 Decisioni Architetturali

### Perché Domain Model Ricco e non Anemic Model?
Le regole di business (es. "non puoi mettere in manutenzione un asset già in manutenzione") vivono nelle entità Domain. Questo garantisce che siano sempre rispettate indipendentemente da chi chiama il codice, rendendo impossibile bypassarle.

### Perché EF Core + Dapper insieme?
EF Core per le operazioni CRUD standard (semplice, type-safe, con migration). Dapper per query complesse dove scrivere SQL diretto è più leggibile ed efficiente (aggregazioni, join multipli, report).

### Perché Result\<T\> invece di eccezioni per i flussi normali?
"Asset non trovato" non è un errore eccezionale — è un flusso normale. Usare `Result.Failure()` rende il flusso esplicito nel codice senza try/catch overhead. Le eccezioni rimangono per errori tecnici davvero imprevisti.

### Perché `timestamp` invece di `timestamptz` in PostgreSQL?
Npgsql 10 richiede che i `DateTime` abbiano `Kind=UTC` per le colonne `timestamp with time zone`. Usando `timestamp` (senza timezone) gestiamo esplicitamente tutti i datetime come UTC nel codice C#, evitando conversioni implicite potenzialmente errate.

---

## 👤 Autore

**Graziano Faraone**
Full Stack .NET Developer — Reggio Emilia, Italia

[![LinkedIn](https://img.shields.io/badge/LinkedIn-Graziano_Faraone-0077B5?style=flat&logo=linkedin)](https://linkedin.com/in/graziano-faraone-26a071218)
[![GitHub](https://img.shields.io/badge/GitHub-faragrazio-181717?style=flat&logo=github)](https://github.com/faragrazio)

Stack: `C#` `ASP.NET Core` `.NET 10` `Entity Framework Core` `PostgreSQL` `JavaScript` `jQuery` `Bootstrap`

---

## 📄 Licenza

Questo progetto è rilasciato sotto licenza MIT. Vedi il file [LICENSE](LICENSE) per i dettagli.
