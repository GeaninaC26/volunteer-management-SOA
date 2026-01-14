# C4 Model Documentation

These diagrams document the system architecture at different levels of abstraction.

## Overview

The C4 model provides four levels of detail:

1. **Context** - System boundaries and external dependencies
2. **Container** - Major runtime components and their interactions
3. **Component** - Internal structure of each container
4. **Code** - Implementation details (covered by UML diagrams)

## C4 Diagrams

### 1. System Context Diagram
**File**: `01-context-diagram.puml`

Shows the system boundary and external actors:

- **Users**: Volunteer coordinators, candidates, volunteers, administrators
- **System**: Volunteer Management System
- **External Dependencies**: Google OAuth 2.0

### 2. Container Diagram
**File**: `02-container-diagram.puml`

Shows runtime containers and communication patterns:

- **Frontend**: Vue.js microfrontends (Host, Campaigns, Registration, Scheduling)
- **NGINX**: Reverse proxy
- **API Gateway**: ASP.NET Core with CQRS
- **Recruitment Service**: Core business logic
- **Notification Service**: SignalR for real-time updates
- **PostgreSQL**: Data persistence
- **RabbitMQ**: Synchronous request/response messaging
- **Kafka**: Asynchronous event streaming

Auxiliary runtime components (used in the actual deployment):

- **Frontend Container**: Builds and serves the Host, Campaigns, Registration, and Scheduling microfrontends
- **Kafka Console**: Redpanda Console for inspecting Kafka topics and messages
- **Init-Data Job**: One-shot container that seeds mock data into the running system

**Data Flow**: Frontend → NGINX → API Gateway → RabbitMQ → Recruitment Service → PostgreSQL. Events are published to Kafka and consumed by the Notification Service for real-time updates.

### 3. Component Diagram - API Gateway
**File**: `03-component-diagram-api-gateway.puml`

Internal structure of the API Gateway:

- **REST Endpoints**: Domain-specific controllers (volunteers, interviews, campaigns with nested candidates/locations/blocked_periods/volunteers, locations, interview_templates, recruitment_form_templates, disponibilities)
- **CQRS Layer**: Command and query handlers with MediatR
- **RabbitMQ Producer**: RPC client with correlation ID tracking
- **Authentication**: JWT, Google OAuth, Cookie-based
- **Authorization**: Policy-based access control

### 4. Component Diagram - Recruitment Service
**File**: `04-component-diagram-recruitment-service.puml`

Internal structure of the Recruitment Service:

- **RabbitMQ Listener**: Message consumer and router
- **Domain Services**: Business logic for candidates, volunteers, interviews, campaigns
- **Data Context**: Entity Framework Core with DbSets
- **Kafka Producer**: Event publisher

**Request Flow**: RabbitMQ Listener → Domain Service → Data Context → PostgreSQL → Kafka Event → Response

### 5. Component Diagram - Notification Service
**File**: `05-component-diagram-notification-service.puml`

Internal structure of the Notification Service:

- **Kafka Consumer**: Event listener
- **Notification Service**: Event-to-notification transformer
- **SignalR Hub**: WebSocket connection manager

**Event Flow**: Kafka Event → Notification Service → SignalR Hub → Frontend Clients

### 6. Deployment Diagram
**File**: `06-deployment-diagram.puml`

Docker Compose deployment configuration:

- **Network**: Bridge network for inter-container communication
- **Exposed Port**: 8080 (NGINX)
- **Service Discovery**: Docker DNS
- **Persistence**: PostgreSQL volume mount
- **Health Checks**: Database and message broker readiness probes
 - **Auxiliary Services**: Frontend build container, Kafka console, and init-data seeding job deployed alongside core services

## Viewing the Diagrams

### Browser
Open [index.html](index.html) for an interactive gallery.

### Generate Images
```bash
plantuml -tpng docs/c4/*.puml
```

### VS Code
1. Install "PlantUML" extension
2. Open `.puml` file
3. Press `Alt+D` (Windows/Linux) or `Option+D` (macOS)

## Diagram Usage by Role

- **Non-technical**: Context diagram
- **Project Managers**: Context and container diagrams
- **Architects**: All diagrams
- **Developers**: Component diagrams
- **DevOps**: Container and deployment diagrams

## C4 vs UML

This project includes both C4 and UML diagrams (`docs/uml/`).

**C4 diagrams** focus on:
- System architecture and structure
- Service communication patterns
- Deployment and infrastructure

**UML diagrams** focus on:
- Detailed class relationships
- Runtime behavior and interactions
- State machines and workflows

Use C4 for architectural understanding, UML for implementation details.

## Architecture Decisions

**Microservices**: Single-responsibility services with independent deployment and scaling.

**CQRS**: Separate read and write models for better performance and maintainability.

**Event-Driven**: Loose coupling through asynchronous event publishing.

**Microfrontends**: Independent frontend modules with on-demand loading via Module Federation.

## Technology Stack

- **Frontend**: Vue.js 3 with Vite
- **Backend**: ASP.NET Core 8.0
- **Database**: PostgreSQL with Entity Framework Core
- **Messaging**: RabbitMQ (RPC), Apache Kafka (Events)
- **Real-time**: SignalR
- **Reverse Proxy**: NGINX
- **Deployment**: Docker Compose

## Related Documentation

- UML diagrams: `docs/uml/`
- API documentation: `/scalar/v1`
- Security guide: `REST_API_Security_Tutorial.md`

## Maintenance

To update diagrams:

1. Edit `.puml` source files
2. Regenerate images: `plantuml -tpng docs/c4/*.puml`
3. Verify diagrams match implementation
4. Update this README as needed
