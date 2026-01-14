````markdown
# Documentation

Complete documentation for the Volunteer Management System.

## UML Diagrams

Detailed UML documentation: [uml](uml/)

### Quick Access
- [View All Diagrams](uml/index.html) - Interactive gallery
- [UML README](uml/README.md) - Complete documentation

### Available Diagrams (11)

**Structural**:
1. Component Diagram - System architecture
2. Domain Model Class Diagram - Business entities
3. API Gateway Class Diagram - CQRS architecture
4. Recruitment Service Class Diagram - Service layer
5. Package Diagram - System layers
6. Deployment Diagram - Docker infrastructure

**Behavioral**:
7. Create Candidate Sequence - CQRS flow
8. Authentication Sequence - OAuth 2.0
9. Schedule Interview Sequence - Business workflow
10. Recruitment Process Activity - End-to-end flow
11. Candidate State Machine - Lifecycle transitions

## C4 Models

Architectural views at different abstraction levels: [c4](c4/)

### Quick Access
- [C4 README](c4/README.md) - Complete documentation

### Available Diagrams (6)

**Level 1 - Context**:
1. Context Diagram - System and external actors

**Level 2 - Containers**:
2. Container Diagram - Runtime components

**Level 3 - Components**:
3. API Gateway Components
4. Recruitment Service Components
5. Notification Service Components

**Deployment**:
6. Deployment Diagram - Docker Compose

## API Documentation

- Swagger/OpenAPI: `/scalar/v1` (when running)
- Security guide: [REST_API_Security_Tutorial.md](../REST_API_Security_Tutorial.md)

## Architecture

- **Pattern**: Service-Oriented Architecture (microservices)
- **Backend**: ASP.NET Core 8.0
- **Frontend**: Vue.js 3 with Module Federation
- **Messaging**: RabbitMQ (RPC), Apache Kafka (Events)
- **Database**: PostgreSQL with Entity Framework Core
- **Real-time**: SignalR
- **Infrastructure**: Docker Compose (including auxiliary services like a Kafka console and an init-data job for seeding mock data)

## Viewing Diagrams

**Browser**:
```bash
open docs/uml/index.html
```

**Generate from source**:
```bash
# Install PlantUML
sudo apt install plantuml  # or: brew install plantuml

# Generate diagrams
plantuml -tpng docs/uml/*.puml
plantuml -tpng docs/c4/*.puml
```

**VS Code**:
1. Install "PlantUML" extension
2. Open `.puml` file
3. Press `Alt+D` to preview

## Maintenance

To update diagrams:

1. Edit `.puml` source files
2. Regenerate: `plantuml -tpng docs/{uml,c4}/*.puml`
3. Verify against implementation
4. Update README files as needed

````

