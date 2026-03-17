# GOATY Mechanic Shop Management System

GOATY is a backend-first **mechanic shop / workshop management system** built with **.NET 9**, **ASP.NET Core Web API**, **Clean Architecture**, and **CQRS with MediatR**.

It models the real daily workflow of a repair shop: customers, vehicles, employees, parts, repair task templates, work orders, scheduling, invoicing, authentication, and manager-facing operational reporting.

---

## Overview

The system is designed around a workshop workflow where a manager can:

- manage customers and their vehicles
- manage employees and technician assignments
- manage parts inventory
- define reusable repair task templates
- create and update work orders
- assign technicians and relocate work orders
- retrieve a daily schedule
- generate and manage invoices
- monitor dashboard metrics
- authenticate users and refresh tokens securely

The project focuses on **real business rules**, **clear module boundaries**, and **maintainable backend architecture** rather than just basic CRUD.

---

## Main features

### Customers & Vehicles

- create, update, delete, and view customers
- attach multiple vehicles to a customer
- prevent duplicate vehicle plates
- enforce customer/vehicle ownership consistency

### Employees

- manage employees
- support role distinctions such as **Manager** and **Technician**

### Parts

- manage the part catalog
- maintain stock quantity and pricing data

### Repair Task Templates

- create reusable repair task definitions
- attach estimated duration, technician cost, and required parts

### Work Orders

- create work orders for specific vehicles and customers
- assign technicians
- relocate work orders to different times / bays
- update work order state
- replace associated repair tasks
- filter, paginate, sort, and search work orders

### Scheduling

- retrieve a daily workshop schedule
- prevent technician conflicts
- prevent bay conflicts
- prevent vehicle overlap

### Billing

- create invoices for work orders
- settle or refund invoices
- generate invoice PDFs

### Dashboard

- retrieve manager-facing dashboard metrics for a specific day

### Identity & Access

- generate JWT access tokens
- refresh access using refresh tokens
- enforce role-based authorization through ASP.NET Core Identity

### Real-time updates

- notify connected clients when work-order collections change
- SignalR hub endpoint: `/hubs/workorders`

---

## Business rules implemented

This project goes beyond controller wiring and includes domain-level rules such as:

- preventing duplicate vehicle plate numbers
- ensuring a vehicle belongs to the selected customer
- preventing technician scheduling conflicts
- preventing workshop bay conflicts
- preventing vehicle time overlap
- enforcing valid work-order state transitions
- restricting edits when a work order is no longer editable
- validating invoice and work-order lifecycle behavior

---

## Technical highlights

- **Clean Architecture**
- **CQRS with MediatR**
- **FluentValidation pipeline behavior**
- **Unhandled exception / performance / logging / caching behaviors**
- **Result / Error pattern** for business outcomes
- **EF Core + SQL Server**
- **ASP.NET Core Identity**
- **JWT + refresh tokens**
- **HybridCache** for cached query flows
- **OpenAPI + Swagger UI + Scalar**
- **QuestPDF** for invoice generation
- **SignalR** for work-order change notifications
- **Background service** for overdue work-order cancellation
- **Audit logging interceptor**
- **ProblemDetails-based error responses**
- **Rate limiting + output caching**
- **Domain unit tests** backed by shared factory helpers

---

## Architecture

The solution follows a layered structure:

- **GOATY.Domain**  
  Core domain model, entities, value objects, enums, domain events, and business rules

- **GOATY.Application**  
  CQRS handlers, validators, DTOs, mappers, interfaces, and MediatR pipeline behaviors

- **GOATY.Infrastructure**  
  EF Core persistence, Identity, JWT, seeding, background jobs, interceptors, PDF generation, SignalR, and supporting services

- **GOATY.Api**  
  ASP.NET Core Web API layer, middleware pipeline, OpenAPI configuration, and controller endpoints

- **GOATY.Contracts**  
  Shared request / contract models

- **GOATY.Client**  
  Blazor web client shell for the upcoming frontend phase

- **GOATY.Common**  
  Shared test factories / helpers used by the unit-test layer

- **GOATY.Domain.UnitTests**  
  Domain-level tests for entities and business rules

---

## Solution structure

```text
GOATY
├── GOATY.Api
├── GOATY.Application
├── GOATY.Client
├── GOATY.Common
├── GOATY.Contracts
├── GOATY.Domain
├── GOATY.Domain.UnitTests
├── GOATY.Infrastructure
└── docs
```
---

## Development-time API surface

Main API modules currently exposed from the API layer include:

- `Customers`
- `Employees`
- `Parts`
- `RepairTasks`
- `WorkOrders`
- `Invoices`
- `Dashboard`
- `Identity`

In development, the API also enables:

- **OpenAPI document** generation
- **Swagger UI**
- **Scalar API reference**
- automatic database initialization + seeding on startup

---

## Seeded development data

On development startup, the initializer creates the database (if needed) and seeds sample data.

### Seeded roles

- `Manager`
- `Technician`

### Seeded users

- **Manager**
  - Email: `a@gmail.com`
  - Password: `a@gmail.com`

- **Technician**
  - Email: `t1@gmail.com`
  - Password: `t1@gmail.com`

### Additional seeded entities

- sample parts
- employees
- customers
- vehicles
- repair tasks
- work orders
- invoices

---

## Documentation

### Project docs

- **PRD:** `docs/PRD.md`
- **Use Case Diagram:** `docs/use-case-diagram.png`

### Activity diagrams

- **Add Work Order:** `docs/activity/Add-WorkOrder-Diagram.png`
- **Add Repair Task:** `docs/activity/Add-Repiar-Task-Diagram.png`
- **Delete Customer:** `docs/activity/Delete-Customer-Diagram.drawio.png`
- **Update Work Order Status:** `docs/activity/Update-WorkOrder-Status.png`
- **View Dashboard:** `docs/activity/View-Dashborad-Diagram.png`

---

## Notes

- The client project is still an early shell and not yet the full production frontend.
- The API is the main implemented surface of the system right now.
- The README is intentionally focused on the current codebase rather than future roadmap ideas.
