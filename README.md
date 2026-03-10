# GOATY Mechanic Shop Management System

A backend-first **workshop / mechanic shop management system** built with **.NET 9**, **ASP.NET Core Web API**, **Clean Architecture**, and **CQRS with MediatR**.

The Project is designed to manage the day-to-day workflow of a repair shop: customers, vehicles, employees, parts, repair task templates, work orders, scheduling, invoices, and authentication. The project focuses on **real business rules and maintainable architecture**.

> Current status: the backend is substantially implemented and the Blazor client exists as a scaffold for the next phase.

---

## Overview

This project models a real workshop workflow where a manager can:

- manage customers and their vehicles
- manage employees and technician assignments
- manage parts inventory
- create reusable repair task templates
- create and update work orders
- schedule technicians and workshop bays
- issue invoices and generate invoice PDFs
- monitor dashboard metrics
- authenticate users with JWT + refresh tokens

The system is built around domain rules such as conflict prevention, valid state transitions, and role-based access.

---

## Main Features

### Core modules

- **Customers & Vehicles**
  - create, update, delete, and view customers
  - attach multiple vehicles to a customer
  - prevent duplicate vehicle plates
  - ensure vehicle ownership rules are respected

- **Employees**
  - manage employees
  - distinguish between **Manager** and **Technician**

- **Parts**
  - manage part catalog and stock quantity
  - track part cost and inventory availability

- **Repair Task Templates**
  - define reusable repair tasks
  - attach estimated duration, technician cost, and required parts

- **Work Orders**
  - create work orders for specific vehicles
  - assign technicians
  - move work orders in time and bay/spot
  - update work order status
  - replace associated repair tasks
  - filter, paginate, sort, and search work orders

- **Scheduling**
  - retrieve a daily technician/workshop schedule
  - prevent scheduling conflicts
  - prevent bay conflicts
  - prevent vehicle overlap

- **Billing**
  - create invoices for work orders
  - mark invoices as settled or refunded
  - generate invoice PDFs

- **Dashboard**
  - retrieve daily dashboard metrics for managers

- **Authentication**
  - JWT access tokens
  - refresh token flow
  - ASP.NET Core Identity integration
  - role-based authorization

---

## Business Rules Implemented

This project goes beyond basic endpoint wiring and includes domain-level rules such as:

- preventing duplicate vehicle plate numbers
- ensuring a vehicle belongs to the selected customer
- preventing technician scheduling conflicts
- preventing workshop bay conflicts
- preventing vehicle time overlap
- enforcing valid work order state transitions
- restricting edits when a work order is no longer editable

---

## Technical Highlights

- **Clean Architecture**
- **CQRS with MediatR**
- **FluentValidation pipeline**
- **Result / Error pattern** for business outcomes
- **EF Core + SQL Server**
- **ASP.NET Core Identity**
- **JWT + refresh tokens**
- **HybridCache** for cached read queries
- **OpenAPI + Swagger UI + Scalar**
- **QuestPDF** for invoice generation
- **Background job** for cancelling overdue scheduled work orders
- **SignalR foundation** for work order change notifications
- **Audit logging interceptor**
- **Domain unit tests** for core entities and business rules

---

## Architecture

The solution follows a layered architecture:

- **GOATY.Domain**  
  Core domain model, entities, value objects, enums, domain events, and business rules

- **GOATY.Application**  
  CQRS handlers, validators, mappings, pipeline behaviors, DTOs, abstractions, and use-case orchestration

- **GOATY.Infrastructure**  
  EF Core persistence, Identity, JWT, seeding, background jobs, interceptors, PDF generation, SignalR, and supporting services

- **GOATY.Api**  
  ASP.NET Core Web API layer and OpenAPI configuration

- **GOATY.Contracts**  
  Shared request/contract models

- **GOATY.Client**  
  Blazor Server client shell for the upcoming frontend phase

---

## Solution Structure

```text
GOATY
??? GOATY.Api
??? GOATY.Application
??? GOATY.Application.UnitTests
??? GOATY.Client
??? GOATY.Contracts
??? GOATY.Domain
??? GOATY.Domain.UnitTests
??? GOATY.Infrastructure
??? docs
??? requests.http

## Documentation

### Project Docs

- **PRD:** `docs/PRD.md`
- **Use Case Diagram:** `docs/use-case-diagram.png`

### Activity Diagrams

- **Add Work Order:** `docs/activity/Add-WorkOrder-Diagram.png`
- **Add Repair Task:** `docs/activity/Add-Repiar-Task-Diagram.png`
- **Delete Customer:** `docs/activity/Delete-Customer-Diagram.drawio.png`
- **Update Work Order Status:** `docs/activity/Update-WorkOrder-Status.png`
- **View Dashboard:** `docs/activity/View-Dashborad-Diagram.png`