# TodoSeUsa - Store Management System

<div align="center">

A complete store management system for family-owned retail businesses.

[![.NET](https://img.shields.io/badge/.NET-9.0-blue?style=flat-square)](https://dotnet.microsoft.com)
[![Blazor](https://img.shields.io/badge/Blazor-Server-512BD4?style=flat-square)](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor)
[![License](https://img.shields.io/badge/License-MIT-green?style=flat-square)](LICENSE)

[Overview](#overview) • [Features](#features) • [Getting Started](#getting-started) • [Project Structure](#project-structure) • [Contact](#contact)

</div>

## Overview

TodoSeUsa is a comprehensive store management system built with **Blazor Server** and **.NET 9**, following **Clean Architecture** principles. Designed for family-owned retail stores, it replaces manual and paper-based workflows by centralizing product management, inventory, providers, consignments, clients, and sales into a single offline-capable application.

The application runs locally on a single Windows machine using a **local SQL Server database** and automatically manages its schema through **Entity Framework Core migrations**.

## Features

- **Product Management** - Catalog and track inventory with images and pricing
- **Provider & Consignment** - Manage supplier relationships and consignment agreements
- **Sales Processing** - Record sales with payment tracking and receipt generation
- **Client Management** - Customer database with contact information
- **Box & Loan System** - Track product boxes and loaned items
- **Payout Management** - Handle payouts to providers and consignors
- **Dashboard** - Monthly sales summary and key business metrics
- **Authentication** - Secure single-user login with recovery code support
- **Offline-First** - Runs without internet connectivity

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [SQL Server Express](https://www.microsoft.com/sql-server/sql-server-downloads) (or SQL Server LocalDB)
- [Visual Studio 2022](https://visualstudio.microsoft.com/downloads/) or [VS Code](https://code.visualstudio.com/)

### Build & Run

```bash
# Clone the repository
git clone <repository-url>

# Navigate to the solution directory
cd TodoSeUsa.App-main

# Restore dependencies
dotnet restore

# Run the application (Development)
cd TodoSeUsa.BlazorServer
dotnet run
```

> [!NOTE]
> In development mode, access the application at `http://localhost:5000`.

### Database Setup

The application automatically creates and migrates the database schema on first run. Ensure SQL Server is running and accessible via the connection string in `appsettings.json`.

### Production Build

```bash
cd TodoSeUsa.BlazorServer
dotnet publish -c Release
```

The published executable can run on any Windows machine as a standalone application.

## Project Structure

```
TodoSeUsa.App/
├── TodoSeUsa.Domain/           # Core business entities and rules
├── TodoSeUsa.Application/      # Business logic, services, DTOs
├── TodoSeUsa.Infrastructure/  # Data access, EF Core, hosting
├── TodoSeUsa.BlazorServer/    # UI layer (Blazor components)
└── DataMigration/             # Legacy data migration tool
```

| Layer | Responsibility |
|-------|----------------|
| **Domain** | Entity definitions, validation rules, business invariants |
| **Application** | Use cases, services, DTOs, interfaces |
| **Infrastructure** | Database context, repositories, file storage |
| **BlazorServer** | Razor components, pages, UI services |

## Demo

Quick overview of the core workflow:

[![Quick view](https://img.youtube.com/vi/TrAAc3k2Blg/maxresdefault.jpg)](https://www.youtube.com/watch?v=TrAAc3k2Blg)

Register & login flow:

[![Login flow](https://img.youtube.com/vi/sZef1EB47fg/maxresdefault.jpg)](https://www.youtube.com/watch?v=sZef1EB47fg)

## Contact

Built by [Matias Margaritini](https://www.linkedin.com/in/matiasmargaritini/)

- Email: [contact.abstr4@gmail.com](mailto:contact.abstr4@gmail.com)
- LinkedIn: [matiasmargaritini](https://www.linkedin.com/in/matiasmargaritini)
- Twitter: [@Abstr4_](https://x.com/Abstr4_)
