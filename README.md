# PortfolioShop: Async Microservices with .NET & RabbitMQ

A high-performance, asynchronously decoupled e-commerce mini-system designed to handle high-throughput traffic (such as flash sales) without blocking user-facing APIs. 

This project demonstrates the transition from traditional monolithic design to an **Event-Driven Architecture (EDA)** using **MassTransit** as a service bus abstraction and **RabbitMQ** as the message broker.

---

## Core Concepts & Key Architectural Benefits

Instead of tightly coupling services with synchronous HTTP calls (which causes cascading failures if a downstream service slows down or crashes), this system utilizes an **Asynchronous Publish-Subscribe (Pub/Sub)** pattern to achieve complete isolation between sub-domains.

* **Temporal Decoupling:** The `OrderApi` does not depend on the availability or performance of the `InventoryService`. It accepts the order payload, hands it off to RabbitMQ, and instantly returns an `HTTP 202 Accepted` status back to the client. This keeps user-facing frontends ultra-responsive.
* **Fault Tolerance & Resiliency:** If the `InventoryService` goes offline for maintenance, or its database crashes under heavy load, messages safely buffer inside RabbitMQ queues. Once the service recovers, it automatically drains the queue and catches up with pending orders without data loss or dropping consumer orders.
* **Horizontal Scalability:** The background processor can easily be scaled horizontally. During high-traffic events (like a flash sale), you can spin up 5 or 10 instances of the consumer worker container, and RabbitMQ will naturally load-balance the message stream across them.

---

## Tech Stack
* **Backend Framework:** .NET 9
* **Message Broker:** RabbitMQ (Advanced Message Queuing Protocol - AMQP)
* **Service Bus Framework:** MassTransit
* **Containerization:** Docker Desktop
* **API Documentation:** Swagger / OpenAPI

---

## Project Structure

* **`PortfolioShop.Contracts`**: A lightweight, shared class library holding the shared message definitions (`public record OrderPlacedEvent`). This allows microservices to share data structures without introducing rigid compile-time codebase dependencies on each other.
* **`PortfolioShop.OrderApi`**: An ASP.NET Core Minimal API acting as the system publisher. It handles edge communication, receives incoming HTTP payloads, and pushes matching event structures onto the message exchange.
* **`PortfolioShop.InventoryService`**: A decoupled .NET Background Worker Service (Console application) acting as the message consumer. It continuously processes incoming queue elements and runs long-running business workflows asynchronously.

---

## Step-by-Step Code Execution

1. **HTTP Entry:** The client hits the `/api/orders` POST endpoint on `OrderApi`. The API processes basic parameters and returns an immediate `HTTP 202 Accepted`.
2. **Event Publishing:** The API maps the request to an `OrderPlacedEvent` record and invokes `publishEndpoint.Publish(orderEvent)`. MassTransit intercepts this call and formats the metadata payload into a JSON packet optimized for transport.
3. **Queueing & Distribution:** RabbitMQ receives the packet at its Fanout exchange, recognizes the binding configuration, and copies the message safely into an isolated storage queue allocated for the `InventoryService`.
4. **Asynchronous Consumption:** The `InventoryService` picks up the message stream. The `OrderPlacedConsumer : IConsumer<OrderPlacedEvent>` handler automatically executes its custom internal system logic, adjusting product counts and streaming safe transaction alerts.

---

## How To Run Locally (Visual Studio Multi-Startup)

1. Start RabbitMQ via Docker
Ensure Docker Desktop is running on your machine, then start up the RabbitMQ broker along with its interactive management UI by running this terminal command:

```bash
docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management
```

2. Launch the Application
Open PortfolioShop.sln in Visual Studio.

Right-click the Solution node in the Solution Explorer -> Select Configure Startup Projects...

Select Multiple Startup Projects and configure both PortfolioShop.OrderApi and PortfolioShop.InventoryService actions to Start. Set PortfolioShop.Contracts to None.

Press F5.

3. Test the Workflow
Locate your API's Swagger page (automatically generated or via https://localhost:[PORT]/swagger).

Open the POST /api/orders action container, click "Try it out", and click Execute with the default testing payload.

Observe the InventoryService console window to see the message instantly parsed, evaluated, and resolved asynchronously in the background.