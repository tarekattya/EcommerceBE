# Ecommerce Backend (EcommerceBE)

[![.NET](https://img.shields.io/badge/.NET-9-blue?logo=dotnet)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/License-MIT-green)](LICENSE)
[![Build Status](https://img.shields.io/badge/build-passing-brightgreen)]()

---

## Table of Contents

1. [Overview](#overview)
2. [Technologies & Tools](#technologies--tools)
3. [Features Implemented](#features-implemented)
4. [Project Structure](#project-structure)
5. [Future Roadmap](#future-roadmap)
6. [How to Run](#how-to-run)
7. [Notes](#notes)
8. [License](#license)

---

## Overview

**EcommerceBE** is a backend application for an e-commerce platform, developed using **.NET 9 / C#** following **Clean Architecture** principles.
It provides a scalable and maintainable backend structure, supporting **product management, brand & category CRUD operations, account & user management, and cart functionality**.
The project is designed for future expansion to handle **orders, checkout, and payment integration**.

---

## Technologies & Tools

* **.NET 10**
* **C#**
* **Entity Framework Core (Code-First Approach)**
* **Clean Architecture** (Domain, Application, Infrastructure, API layers + Shared layer)
* **Mapster** for DTO and Entity mapping
* **Redis** for cart management
* **JWT Authentication & Identity** (planned for authentication and role management)

> Note: The **Shared layer** contains reusable components, DTOs, helpers, and abstractions used across all layers.

---

## Features Implemented

### 1. Products

* CRUD operations (Create, Read, Update, Delete)
* Mapping with Brand, Category, and Picture URL
* Pagination, sorting, and filtering support

### 2. Brands & Categories

* Full CRUD operations

### 3. Users & Accounts

* Basic user management
* Integration with **Identity** for authentication & roles (planned)

### 4. Cart

* Add, update, delete items
* Redis-based storage for fast access

### 5. Specifications

* Product specifications support (filters, pagination, sorting)
* Querying products with brand & category inclusion

---

## Project Structure

```
EcommerceBE/
├─ Ecommerce.Core/             # Domain entities, interfaces, specifications
├─ Ecommerce.Application/      # Business logic, services, DTOs, helpers
├─ Ecommerce.Infrastructure/   # Data access, EF Core, Redis implementations
├─ Ecommerce.Shared/           # Shared helpers, DTOs, common abstractions
├─ Ecommerce.API/              # Controllers, mapping, configuration, program entry
```

---

## Future Roadmap

* **Authentication & Authorization**

  * JWT-based login & registration
  * Role management (Admin, User)

* **Orders & Checkout**

  * Create, view, and manage orders
  * Payment gateway integration

* **Dashboard (MVC)**

  * Admin dashboard for managing products, brands, categories, and users

* **Notifications & Emails**

  * Order confirmations, shipping updates

* **Advanced Features**

  * Product reviews & ratings
  * Wishlist management
  * Analytics & reporting

---

## How to Run

1. Clone the repository:

```bash
git clone https://github.com/tarekattya/EcommerceBE.git
```

2. Open the **.sln file** in **Visual Studio 2022+** or **VS Code**

3. Apply migrations (from solution root / Infrastructure layer detected automatically):

```bash
dotnet ef database update
```

4. Run the API project (`Ecommerce.API`)

5. Access endpoints via **Postman** or **Swagger UI**

---

## Notes

* Ensure **Redis** is running for cart management
* Mapster configuration must be registered in **Program.cs**
* Default values for pagination are set inside **ProductSpecParams** to handle missing query parameters
* Shared layer contains **reusable DTOs and helper methods**

---

## License

This project is licensed under the **MIT License** – see the [LICENSE](LICENSE) file for details.
