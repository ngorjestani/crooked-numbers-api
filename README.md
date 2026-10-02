# Crooked Numbers API

Typed ASP.NET Core integration API for the Crooked Numbers web application. The initial
version reads standard baseball information from the public MLB Stats API and does not
require a database, authentication, or secrets.

## Prerequisites

- .NET 10 SDK (the current LTS; see `global.json`)

## Configuration

Defaults live in `src/CrookedNumbers.Api/appsettings.json`:

- `MlbApi:BaseUrl` — MLB Stats API root URL.
- `MlbApi:*CacheMinutes` — in-memory freshness windows by resource.
- `Cors:AllowedOrigins` — browser origins allowed to call the API.

Override any value with an environment variable using ASP.NET Core's double-underscore
notation, for example `Cors__AllowedOrigins__0=http://localhost:4173`.

## Run locally

```bash
dotnet restore
dotnet run --project src/CrookedNumbers.Api
```

The HTTP development profile listens on `http://localhost:5088`. In development, the
OpenAPI document is available at `http://localhost:5088/openapi/v1.json`.

## Validation

```bash
dotnet build
dotnet test
dotnet tool restore
dotnet csharpier check .
```

Run `dotnet csharpier format .` to apply C# formatting.

## Endpoints

- `GET /api/teams`
- `GET /api/teams/{teamId}`
- `GET /api/teams/{teamId}/roster`
- `GET /api/teams/{teamId}/stats`
- `GET /api/teams/{teamId}/schedule?startDate=YYYY-MM-DD&endDate=YYYY-MM-DD`

Schedule windows default to 14 days before and after today and are limited to 93 days.
Responses use Crooked Numbers contracts rather than exposing MLB response objects.

## MLB dependency

The API consumes these MLB Stats API resources:

- `/api/v1/teams` and `/api/v1/teams/{id}`
- `/api/v1/teams/{id}/roster`
- `/api/v1/stats` for season hitting and pitching player splits
- `/api/v1/schedule`

MLB calls use a typed `HttpClient`; tests replace HTTP or the client boundary and do not
make live network requests.
