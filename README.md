# GOATY Mechanic Shop (WIP)

A starter **Mechanic Shop / Workshop Management System** built on **.NET 9** with a **Clean Architecture** structure and **CQRS** (via MediatR).  

---

## What’s inside

- **GOATY.Api** ? ASP.NET Core Web API (presentation layer)
- **GOATY.Client** ? Blazor Server UI (Razor Components)
- **GOATY.Domain** ? Core business rules (Entities, Domain Events, Result/Error pattern)
- **GOATY.Application** ? CQRS (Commands/Queries + handlers)
- **GOATY.Infrastructure** ? DB / persistence / external services
- **GOATY.Contracts** ? Shared DTOs/contracts between API & Client 

---

## Architecture (Clean Architecture vibe)

The idea is simple:

- **Domain** stays pure: business logic only.
- **Application** orchestrates use-cases using **CQRS** (Commands/Queries) and MediatR.
- **Infrastructure** plugs in implementations (DB, files, integrations).
- **API/Client** are just entry points (HTTP/UI) — they call the Application layer.

---

## Run the API

```bash
dotnet restore
dotnet run --project GOATY.Api
