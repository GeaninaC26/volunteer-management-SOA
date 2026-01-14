# UML Documentation

These diagrams document the system architecture, design patterns, and runtime behavior.

## Overview

The system is a microservices application for managing volunteer recruitment:

- **Frontend**: Vue.js microfrontends
- **Backend**: ASP.NET Core 8.0 microservices
- **Messaging**: RabbitMQ (RPC), Apache Kafka (Events)
- **Database**: PostgreSQL with Entity Framework Core
- **Real-time**: SignalR
- **Infrastructure**: Docker Compose

## UML Diagrams

### 1. Component Diagram
**File**: `01-component-diagram.puml`

System architecture overview:
- **Frontend**: Host, Campaigns, Registration, Scheduling microfrontends
- **Backend**: API Gateway, Recruitment Service, Notification Service
- **Infrastructure**: NGINX, RabbitMQ, Kafka, PostgreSQL
- **Communication**: RabbitMQ (RPC), Kafka (Events), SignalR (Real-time)

### 2. Domain Model - Class Diagram
**File**: `02-class-diagram-domain-model.puml`

Core domain entities and relationships:
- **RecruitmentCampaign**: Aggregate root
- **Candidate**: Application records
- **Volunteer**: Registered volunteers
- **Interview**: Scheduled interviews
- **Supporting Entities**: Location, BlockedPeriod, InterviewTemplate, Disponibility

**Relationships**: Campaign (1) → (N) Candidates, Volunteers, Locations. Interview (N) ↔ (N) Volunteers.

### 3. Create Candidate Sequence Diagram
**File**: `03-sequence-diagram-create-candidate.puml`

Request flow for creating a candidate:
1. Frontend submits request
2. API Gateway authenticates and creates CQRS command
3. RabbitMQ RPC to Recruitment Service
4. Database transaction
5. Kafka event publication
6. SignalR notification to frontend

**Patterns**: CQRS, RPC with correlation IDs, Event-Driven Architecture

### 4. Authentication Flow Sequence Diagram
**File**: `04-sequence-diagram-authentication.puml`

Google OAuth 2.0 authentication:
1. Login initiation
2. Google OAuth redirect
3. Authorization code exchange
4. Token validation
5. Session creation (cookie-based)
6. Role assignment

**Security**: JWT Bearer, secure cookies, role-based authorization

### 5. Deployment Diagram
**File**: `05-deployment-diagram.puml`

Docker Compose infrastructure:
- **Network**: Bridge network (volunteer-network)
- **Ports**: NGINX (8080), RabbitMQ (5672, 15672), Kafka (9092, 9094)
- **Persistence**: PostgreSQL volume mounts
- **Health Checks**: Database and message broker readiness
- **Communication**: Service discovery via Docker DNS

### 6. API Gateway Class Diagram
**File**: `06-class-diagram-api-gateway.puml`

API Gateway internal structure:
- **Endpoints**: REST controllers per domain (volunteers, interviews, campaigns with nested resources, locations, templates, forms, disponibilities),
  plus supporting endpoints for enum/type lookups and authentication (`/login`, `/logout`, `/userinfo`).
- **CQRS**: Command and query separation
- **MediatR**: Command/query routing
- **Messaging**: RabbitMQ RPC producer
- **Authentication**: JWT, Google OAuth, Cookie

**Patterns**: CQRS, Mediator, Repository

### 7. Recruitment Service Class Diagram
**File**: `07-class-diagram-recruitment-service.puml`

Recruitment Service internal structure:
- **RabbitMQListener**: Message consumer
- **Domain Services**: Business logic per entity
- **DataContext**: Entity Framework Core
- **KafkaProducer**: Event publisher

**Layers**: Messaging → Service → Data Access → Domain Model

### 8. Schedule Interview Sequence Diagram
**File**: `08-sequence-diagram-schedule-interview.puml`

Interview scheduling workflow:
1. Coordinator selects candidate, time, location, interviewers
2. API Gateway routes command
3. Business validations:
   - Volunteer availability
   - Blocked periods
4. Database transaction
5. Event publication
6. Notifications sent

**Enforced Rules**: Availability verification, blocked period checks

### 9. Recruitment Process Activity Diagram
**File**: `09-activity-diagram-recruitment-process.puml`

End-to-end recruitment workflow:
1. Create campaign
2. Candidate applications and volunteer availability registration (parallel)
3. Application review
4. Interview scheduling
5. Interview execution
6. Hiring decision
7. Volunteer creation (if accepted)
8. Campaign completion

**Decision Points**: Application validity, qualification, availability, acceptance

### 10. Candidate State Machine Diagram
**File**: `10-state-diagram-candidate.puml`

Candidate lifecycle states:
- **Open**: Initial application
- **Scheduled**: Interview scheduled
- **Pending**: Interview completed, awaiting decision
- **Accepted**: Approved
- **Rejected**: Declined

**Transitions**: Open → Scheduled/Rejected, Scheduled → Pending, Pending → Accepted/Rejected, Accepted → Volunteer

**Kafka Topics Used**: Events related to candidates, campaigns, and scheduling are published to Kafka topics such as
`candidate_updates`, `campaign_updates`, `location_updates`, `interview_template_updates`,
`recruitment_form_template_updates`, `campaign_volunteer_updates`, and `schedule_updates`.

### 11. Package Diagram
**File**: `11-package-diagram.puml`

System layered structure:

- **Frontend**: Host, Campaigns, Registration, Scheduling
- **API**: API Gateway (endpoints, CQRS, messaging, auth)
- **Services**: Recruitment Service, Notification Service
- **Shared**: Domain entities, DTOs, enums
- **Infrastructure**: RabbitMQ, Kafka, PostgreSQL, NGINX

**Dependencies**: Frontend → NGINX → Gateway → RabbitMQ → Recruitment → PostgreSQL/Kafka → Notification → Frontend

## Viewing the Diagrams

### Browser
Open [index.html](index.html) for an interactive gallery.

### Generate Images
```bash
plantuml -tpng docs/uml/*.puml
```

### VS Code
1. Install "PlantUML" extension
2. Open `.puml` file
3. Press `Alt+D` (Windows/Linux) or `Option+D` (macOS)

## Architecture Patterns

**CQRS**: Command/query separation with MediatR in API Gateway

**Event-Driven**: Domain events published to Kafka after state changes

**SOA**: API Gateway (routing), Recruitment Service (business logic), Notification Service (real-time)

**Messaging**: RabbitMQ RPC (synchronous), Kafka pub/sub (asynchronous)

**Microfrontends**: Module Federation with shared authentication state

## Technology Stack

- **Frontend**: Vue.js 3, Vite, Module Federation
- **Backend**: ASP.NET Core 8.0, Entity Framework Core, MediatR
- **Messaging**: RabbitMQ (RPC), Apache Kafka (Events)
- **Database**: PostgreSQL
- **Real-time**: SignalR
- **Gateway**: NGINX
- **Deployment**: Docker Compose

## Related Documentation

- C4 model diagrams: `docs/c4/`
- API documentation: `/scalar/v1`
- Security guide: `REST_API_Security_Tutorial.md`

## Maintenance

To update diagrams:

1. Edit `.puml` source files
2. Regenerate images: `plantuml -tpng docs/uml/*.puml`
3. Verify diagrams match implementation
4. Update this README as needed
