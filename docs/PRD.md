# Product Requirements Document (PRD)

- **Product:** Mechanic Shop Management System  
- **Version:** 1  
- **Date:** 16/02/2026  

---

## Product Overview

The **Mechanic Shop Management System** is an operational platform designed to manage:

1. **Customers**
2. **Customers’ Vehicles**
3. **Labor (Technician) Assignment**
4. **Operations Monitoring**
5. **Scheduling**
6. **Repair Tasks**
7. **Work Orders**

It supports day-to-day repair shop tasks by **centralizing a single source of truth** for customers, vehicles, work order tracking, technician scheduling, and service execution. It handles scheduling conflicts, provides a clear visible task plan, and helps deliver consistent, high-quality daily service.

---

## Problem Statement — Why This System Exists

Many repair shops still rely on paper notes or human memory to manage daily operations. This creates issues such as:

- **Scheduling failures:** missed or delayed customer appointments due to weak scheduling and no centralized calendar  
- **Technician conflicts:** technicians get double-booked or assigned to overlapping jobs  
- **Customer history is hard to retrieve:** searching for past customer/vehicle/job records is slow or impossible  
- **Low operational visibility:** hard to track daily progress (what’s scheduled, in progress, completed, cancelled)

---

## Goals

1. **Eliminate Paper-Based Work:** Digitize customer, vehicle, and work order data so nothing is lost and everything is searchable.  
2. **24/7 Operational Visibility:** Track work orders, schedules, and technician workload in real time so the shop always knows what’s happening.  
3. **Prevent Overlapping Conflicts:** Stop double-booking by enforcing scheduling rules for technicians and service bays.  
4. **Fast Customer & Vehicle Data Retrieval:** Find any customer, vehicle, or past job history instantly.  
5. **Simplify Scheduling:** Provide an easy scheduling workflow (daily view + technician view) that makes planning quick and clear.  
6. **Increase Service Quality & Efficiency:** Standardize repair tasks using templates to reduce mistakes and speed up completion.  
7. **Enable Better Decisions Through Tracking & Reporting:** Provide basic metrics (Scheduled / InProgress / Completed / Cancelled) to support planning and management.

---

## Roles & Permissions

| Role | Permissions |
| --- | --- |
| **Manager** | Full system access: manage customers, vehicles, repair tasks, work orders, scheduling, issue invoices, and view all reports |
| **Labor (Technician)** | Limited access: view assigned work orders, update work order status to **InProgress** or **Completed** |

---

## Functional Requirements

### Customers & Vehicles Management

#### Manager Can
1. View current customer information  
2. Add new customer information  
3. Add a new vehicle to a customer  
4. Update customer or vehicle information  
5. Delete customer or vehicle records  

#### Business Rules
1. You cannot remove a customer who has a work order (Scheduled / InProgress).  
2. Removing an existing customer will also remove all of their vehicles.  

#### Acceptance Criteria
1. New customers must have valid information  
2. Customer ID must be unique  
3. Vehicle plate number must be unique  

---

### Repair Task Templates

#### Manager Can
1. View current template information  
2. Create a new repair task template (including needed parts)  
3. Set estimated time and cost for a template  
4. Update template information  
5. Delete templates  

#### Business Rules
1. Updating a template does not affect existing work orders that use that template  
2. Deleting a template does not affect existing work orders that use that template  
3. Parts can be added/removed from a template  
4. A template can be created even if specific parts are not available  

#### Acceptance Criteria
1. Estimated time and cost must be positive values  
2. Template name must be unique  

---

### Work Order Management

#### Manager Can
1. View all work orders  
2. Create a new work order  
3. Select/assign a customer to a work order  
4. Select/assign a vehicle to a work order  
5. Assign **one technician** to a work order  
6. Add one or more repair task templates to a work order  
7. Update work order details (customer, vehicle, technician, templates, scheduled time)  
8. Update work order status  
9. Delete a work order (if allowed)  

#### Technician Can
1. View only their assigned work orders  
2. Update the status of their assigned work orders  

#### Work Order Statuses
- Scheduled  
- InProgress  
- Completed  
- Cancelled  

#### Valid Status Transitions
- Scheduled → InProgress  
- InProgress → Completed  
- Scheduled → Cancelled  
- InProgress → Cancelled  

#### Business Rules
1. A work order can have only one technician assigned.  
2. If the customer does not show up, the work order is auto-cancelled after 20 minutes (only if still Scheduled).  
3. A work order cannot be deleted while it is InProgress.  

#### Acceptance Criteria
1. When a manager creates a work order, its default status is Scheduled.  
2. A technician can only see work orders assigned to them.  
3. A technician can only update the status of work orders assigned to them.  
4. If the scheduled time passes by 20 minutes and the work order is still Scheduled, the system automatically sets it to Cancelled and marks it as “No-show”.  
5. If a manager tries to delete a work order in InProgress, the system blocks it with a clear message.  

---

### Scheduling

#### Manager Can
1. Schedule based on date, time, and service bay  
2. View daily schedules  
3. Assign a technician to a work order  
4. Update scheduling (time, technician, service bay)  
5. Remove a scheduled work order  

#### Technician Can
1. View their schedule  

#### Business Rules
1. No conflicts allowed  
2. No double booking (technician or service bay overlap)  
3. Cannot remove/unassign technician while the work order is InProgress  
4. You can update the schedule while it’s InProgress, but only limited fields:  
   - Allowed: change service bay (only if no conflict)  
   - Not allowed: change time or technician  

#### Acceptance Criteria
- Manager can schedule a work order with date/time/bay, and it appears in the daily schedule view  
- If a scheduling conflict happens (technician or bay overlap), the system rejects it with a clear message  
- Technician can only see their assigned schedule  
- If a work order is InProgress, manager cannot remove the technician from it  
- If a work order is InProgress, manager cannot change its time/technician, but can change the service bay if no conflict  

---

### Labor Management

#### Manager Can
1. View list of all technicians  
2. See technician availability for scheduling  
3. Assign technicians to work orders  
4. Reassign work orders if needed  

#### Acceptance Criteria
1. Cannot double-book a technician  
2. System prevents overlapping assignments  
3. Technicians can only see their own assigned work orders  

---

### Dashboard & Reporting

#### Manager Can
1. View work order statistics by date  
2. See total work orders (all states)  
3. See count of completed work orders  
4. See count of in-progress work orders  
5. See count of cancelled work orders  

#### Acceptance Criteria
1. Dashboard updates in real time or near real time  
2. Can filter statistics by date range  
3. Statistics are accurate and match work order data  

---

### Authentication & Authorization

#### User Can
1. Log in with username and password  
2. Remain logged in for a reasonable session duration  
3. Log out when finished  

#### Security Requirements
1. All pages except login require authentication  
2. Managers can access all features  
3. Technicians can only access their assigned work orders  
4. Passwords must be securely stored (never in plain text)  
5. Sessions must expire after a period of inactivity  
6. Failed login attempts should be rate-limited  

#### Acceptance Criteria
1. Unauthenticated users are redirected to the login page  
2. Invalid credentials show a clear error message  
3. Users see only the features they have permission to access  
4. Session persists across page refreshes  
5. Logout clears the session completely  

---

## Non-Functional Requirements

### Performance
- Page load time < 2 seconds under normal conditions  
- API response time < 500ms for standard queries  
- Supports at least 10 concurrent users without degradation  
- Database queries optimized to prevent slow operations  

### Availability
- System available during business hours (6 AM - 8 PM)  
- Planned maintenance communicated in advance  
- Data backed up daily  

### Usability
- Intuitive interface requiring minimal training  
- Clear error messages that guide users to resolution  
- Responsive design (desktop + tablet)  
- Consistent navigation across all pages  

### Security
- Sensitive data encrypted in transit (HTTPS)  
- Passwords must meet minimum complexity requirements  
- Sessions expire after 30 minutes of inactivity  
- Audit log of all data modifications (who, what, when)  

### Data Integrity
- All database operations must be transactional  
- Prevent invalid data entry through validation  
- Referential integrity maintained  
- Data recovery possible from daily backups  

### Maintainability
- Consistent code style guidelines  
- APIs documented with clear descriptions  
- Business logic separated from data access  
- Automated tests for critical workflows  

---

## Acceptance Criteria Summary

### Must Have (P0)
- [ ] User authentication and role-based access  
- [ ] Customer and vehicle CRUD operations  
- [ ] Repair task catalog management  
- [ ] Work order lifecycle management  
- [ ] Scheduling with conflict prevention  
- [ ] Auto-cancellation of no-shows  
- [ ] Basic dashboard with statistics  

### Should Have (P1)
- [ ] Audit logging of changes  
- [ ] Advanced search and filtering  
- [ ] Data export capabilities  

### Could Have (P2)
- [ ] Reporting and analytics  
- [ ] Email notifications  
- [ ] Calendar view of schedule  

### Won’t Have
- Payment processing  
- Inventory management  

---

## Glossary

| Term | Definition |
| --- | --- |
| **Work Order** | A job request to perform repair work on a specific vehicle |
| **Repair Task** | A reusable template defining a standard service procedure |
| **Labor** | A technician who performs repair work |
| **Service Bay** | A physical location where repair work is performed |
| **Schedule** | The allocation of work orders to specific times and service bays |
| **No-Show** | When a customer fails to arrive for their scheduled appointment |

---

## Document Metadata

- **Document Version:** 1  
- **Last Updated:** 19/02/2026  
- **Document Owner:** Ameer Tamimi  
- **Stakeholders:** Only me for now .. hopefully real customers in the future :( 
