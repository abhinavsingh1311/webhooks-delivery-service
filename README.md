# Reliable Webhook Delivery Service

![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=flat&logo=dotnet)
![C#](https://img.shields.io/badge/C%23-13.0-239120?style=flat&logo=csharp)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-9.0-512BD4?style=flat&logo=dotnet)
![SQL Server](https://img.shields.io/badge/SQL%20Server-2019+-CC2927?style=flat&logo=microsoftsqlserver)
![Entity Framework Core](https://img.shields.io/badge/EF%20Core-9.0-512BD4?style=flat&logo=dotnet)
![Docker](https://img.shields.io/badge/Docker-Supported-2496ED?style=flat&logo=docker)
![License](https://img.shields.io/badge/License-MIT-green?style=flat)

A durable webhook delivery system built with .NET 9 and SQL Server that guarantees at-least-once delivery with automatic retries and exponential backoff.

[Live Demo](https://webhook-delivery-b6bshhhtg4gyf5gm.canadacentral-01.azurewebsites.net)

## Table of Contents

- [About](#about)
- [Features](#features)
- [Tech Stack](#tech-stack)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
- [Configuration](#configuration)
- [API Reference](#api-reference)
- [License](#license)

## About

This service solves the problem of reliable event delivery to external endpoints. When your system needs to notify third-party services about events, network failures, timeouts, and service outages can cause lost notifications. This project implements a durable queue pattern in SQL Server with retry logic to ensure webhooks are delivered even when failures occur.

**Problem it solves:** Unreliable webhook delivery leading to data inconsistency between systems.

**Approach:** Persist events before acknowledging, process asynchronously with retries, and move permanently failed events to a dead letter queue for manual inspection.

## Features

- [x] Webhook endpoint registration with secret management
- [x] Durable event persistence before acknowledgment
- [x] Automatic retries with exponential backoff and jitter
- [x] HMAC-SHA256 payload signing for receiver verification
- [x] Dead letter queue for failed deliveries
- [x] Delivery status tracking and reporting
- [x] Concurrent processing with row-level locking

## Tech Stack

| Layer | Technology |
|-------|------------|
| Framework | .NET 9 |
| Web | ASP.NET Core Web App |
| Database | SQL Server |
| ORM | Entity Framework Core 9 |
| Background Jobs | IHostedService |
| Testing | xUnit, Testcontainers |

## Project Structure

```
|---WebHooks/   # ASP.NET Core Web App (UI + API)
|       |
|       |--- Components/
|       |         |--- Pages/
|       |         |      |--- Events.razor + .cs (partial class)
|       |         |      |___ Endpoints.razor + .cs (partial class)
|       |         |      |___ Home.razor
|       |         |---- Layout/  
|       |---- Workers/
|       |         |--- DeliveryWorker.cs
|       |---- Program.cs
|
├── WebHooks-System-Library/       # Class Library (BLL, DAL, DbContext)
│       ├── Data/
│       │   └── WebhooksDeliveryContext.cs
│       ├── Entities/
│       │   ├── WebhookEp.cs
│       │   |── WebhookEvent.cs
|       |   |-- WebhookStatus.cs
│       ├── Repositories/
|       |   ├── IDeliveryService.cs
│       │   └── IWebhookRepo.cs
│       └── Services/
│           ├── DeliveryService.cs
│           └── WebHookService.cs
├── tests/
│   └── WebHooks.Tests/ (future implementation)
└── README.md
```

## Getting Started

### Prerequisites

- .NET 9 SDK
- SQL Server 2019+ or SQL Server LocalDB
- Docker (optional, for containerized SQL Server)

### Installation

1. Clone the repository
```bash
git clone https://github.com/abhinavsingh1311/webhook-delivery-service.git
cd webhook-delivery-service
```

2. Set up the database
```bash
cd WebHooks
dotnet ef database update
```

3. Run the application
```bash
dotnet run
```

The application will be available at `https://localhost:7068`.

## Configuration

Update `appsettings.json` with your settings:

```json
{
   "ConnectionStrings": {
   "WebhooksDeliveryDatabase": "Server=.;Database=Webhooks-Delivery;Trusted_Connection=true;TrustServerCertificate=true;"
 },
}
```

## API Reference

### Register Endpoint
```
POST /api/endpoints
```

### Send Event
```
POST /api/events
```

### Check Delivery Status
```
GET /api/events/{id}/status
```

### View Dead Letter Queue
```
GET /api/deadletter
```

## License

Distributed under the MIT License. See `LICENSE` for more information.
