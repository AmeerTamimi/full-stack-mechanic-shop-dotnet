# GOATY Mechanic Shop (WIP)

A **Mechanic Shop / Workshop Management System** built with **.NET 9**, **Clean Architecture**, and **CQRS** using **MediatR**.

The project is designed as a layered backend-first system for managing **customers, vehicles, employees, parts, repair task templates, work orders, scheduling, and authentication**. It goes beyond basic CRUD by including **real business rules, validation, caching, and role-based workflow structure**.

## Docs

- **PRD:** `docs/PRD.md`
- **Use Case Diagram:** `docs/use-case-diagram.png`

## Activity Diagrams

- **Add Work Order** — `docs/activity/Add-WorkOrder-Diagram.png`
- **Add Repair Task** — `docs/activity/Add-Repiar-Task-Diagram.png`
- **Delete Customer** — `docs/activity/Delete-Customer-Diagram.drawio.png`
- **Update Work Order Status** — `docs/activity/Update-WorkOrder-Status.png`
- **View Dashboard** — `docs/activity/View-Dashborad-Diagram.png`

## What’s implemented

- Customer and vehicle management
- Employee management
- Parts management
- Repair task template management
- Work order creation and updates
- Work order filtering, pagination, and sorting
- JWT authentication + refresh tokens
- ASP.NET Core Identity integration
- Domain unit tests for core entities

## Business rules included

- Prevent duplicate vehicle plates
- Ensure vehicle belongs to the selected customer
- Prevent technician scheduling conflicts
- Prevent bay conflicts
- Prevent vehicle time overlap
- Enforce valid work order state transitions
- Restrict editing when a work order is no longer editable

## Architecture

- **GOATY.Domain** — core business rules and domain model
- **GOATY.Application** — CQRS handlers, validators, mappings, behaviors
- **GOATY.Infrastructure** — EF Core, Identity, JWT, persistence, seeding
- **GOATY.Api** — ASP.NET Core Web API
- **GOATY.Contracts** — shared contracts/DTOs
- **GOATY.Client** — Blazor Server UI shell

## Technical highlights

- Clean Architecture separation
- CQRS with MediatR
- FluentValidation pipeline
- Result/Error pattern for business outcomes
- Logging, performance, and exception behaviors
- HybridCache for cached read queries
- EF Core + SQL Server
- Role foundation for **Manager** and **Technician**

## Why it stands out

This project shows more than endpoint building. It demonstrates **architecture thinking, separation of concerns, enforceable business logic, and a scalable structure** for a real workshop management system.

## Run

```bash
dotnet restore
dotnet run --project GOATY.Api