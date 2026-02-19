# GOATY Mechanic Shop (WIP)

A starter **Mechanic Shop / Workshop Management System** built on **.NET 9** with **Clean Architecture** and **CQRS** (via MediatR).

---

## Docs

- **PRD (Product Requirements Document):** `docs/PRD.md`
- **Use Case Diagram :** `docs/use-case-diagram.png`

---
## Product & Design Docs

- **PRD (Product Requirements Document):** `docs/PRD.md`
- **Use Case Diagram:** `docs/use-case-diagram.png`

### Activity Diagrams (Swimlanes)

These diagrams document the **main flows + alternatives** (business rules, validation, and permission boundaries):

1. **Add New Work Order Flow**  
   File: `docs/activity/Add-WorkOrder-Diagram.png`  
   Covers: create work order, conflict check (technician overlap), success vs error path.

2. **Add Repair Task Flow**  
   File: `docs/activity/Add-Repiar-Task-Diagram.png`  
   Covers: add template, **unique name** validation, error handling.

3. **Delete Customer Flow**  
   File: `docs/activity/Delete-Customer-Diagram.drawio.png`  
   Covers: block deletion if customer has **Scheduled/InProgress** work order, otherwise delete customer + his vehicles.

4. **Update Work Order Status Flow (Technician)**  
   File: `docs/activity/Update-WorkOrder-Status.png`  
   Covers: technician-only access, **valid status transitions**, reject invalid transitions.

5. **View Dashboard Flow (Manager)**  
   File: `docs/activity/View-Dashborad-Diagram.png`  
   Covers: dashboard metrics + optional filtering.

1. 
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
