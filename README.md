# CvWebApi

## Overview
CvWebApi is a .NET Core Web API designed for managing and displaying CV-related data, including cards, badges, and carousel functionality. The project follows Onion Architecture to ensure modularity, maintainability, and separation of concerns.

## Features
- Implements **Onion Architecture** to maintain a clean separation between layers.
- Supports **CRUD operations** for Cards and Carousels.
- Uses **Entity Framework Core (EF Core)** for data persistence.
- Includes **AutoMapper** for DTO conversions.
- Supports **transactions** to maintain data consistency.

## Project Structure
The solution consists of multiple class libraries:

- **CvWebApi (API Layer)**: The main entry point containing controllers and routing logic.
- **Integration (Repository Layer)**: Manages data access and interactions with the database.
- **Services (Business Logic Layer)**: Contains business rules and operations.
- **Context (Data Layer)**: Defines the EF Core DbContext and entity models.

## Technologies Used
- **.NET Core 6/7**
- **Entity Framework Core**
- **AutoMapper**
- **SQL Server** (or any compatible database)
- **Swagger/OpenAPI** (for API documentation)

## Installation & Setup
### Prerequisites
- .NET SDK (6.0/7.0 or later)
- SQL Server (or another configured database)

### Steps to Run Locally
1. Clone the repository:
   ```sh
   git clone https://github.com/Kaminari-Mizu/CvWebApi.git
   ```
2. Navigate to the project directory:
   ```sh
   cd CvWebApi
   ```
3. Set up the database:
   ```sh
   dotnet ef database update
   ```
4. Run the API:
   ```sh
   dotnet run
   ```
5. Open Swagger UI:
   ```
   http://localhost:<port>/swagger
   ```

## API Endpoints
### Card Endpoints
- **POST** `/api/cards` - Create a new card
- **GET** `/api/cards/{id}` - Retrieve a card by ID
- **PUT** `/api/cards/{id}` - Update an existing card
- **DELETE** `/api/cards/{id}` - Delete a card

### Carousel Endpoints
- **POST** `/api/carousels` - Create a new carousel
- **GET** `/api/carousels/{id}` - Retrieve a carousel by ID
- **PUT** `/api/carousels/{id}` - Update a carousel
- **DELETE** `/api/carousels/{id}` - Delete a carousel

## Contribution
Contributions are welcome! Please follow these steps:
1. Fork the repository
2. Create a new branch (`feature-branch`)
3. Commit changes
4. Open a pull request


