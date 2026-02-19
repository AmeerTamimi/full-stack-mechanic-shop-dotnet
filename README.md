# GOATY Mechanic Shop (WIP)

A starter **Mechanic Shop / Workshop Management System** built on **.NET 9** with **Clean Architecture** and **CQRS** (via MediatR).

---

## Docs

- **PRD (Product Requirements Document):** `docs/PRD.md`
- **Use Case Diagram :** `docs/use-case-diagram.png`

---

## What’s inside

- **GOATY.Api** : ASP.NET Core Web API (presentation layer)
- **GOATY.Client** : Blazor Server UI (Razor Components)
- **GOATY.Domain** : Core business rules (Entities, Domain Events, Result/Error pattern)
- **GOATY.Application** : CQRS (Commands/Queries + handlers)
- **GOATY.Infrastructure** : DB / persistence / external services
- **GOATY.Contracts** : Shared DTOs/contracts between API & Client

---

## Architecture (Clean Architecture)

- **Domain** stays pure: business logic only.
- **Application** orchestrates use-cases using **CQRS** + MediatR.
- **Infrastructure** plugs in implementations (DB, files, integrations).
- **API/Client** are entry points (HTTP/UI) — they call the Application layer.

---

## Run the API

```bash
dotnet restore
dotnet run --project GOATY.Api
