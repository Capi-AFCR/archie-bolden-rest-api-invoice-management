RestAPIInvoiceManagement

Overview

		RestAPIInvoiceManagement is a .NET 9.0 Web API for managing invoices, clients, payments, and user authentication. 
		It uses SQLite as the database, implements API versioning (/api/v1/), and secures endpoints with JWT authentication. 
		The API supports CRUD operations for clients and invoices, payment management, and user registration/login. 
		The project includes unit tests and is containerized with Docker for easy deployment.

Use of AI
		
		It was used to know the console commands to create the base project (without code included), and how to run the API.
		It was used for investigate causes of errors regarding <T> types, and also gave me light about some errors i had in my unit tests.
		It was used for some clarifications about Docker installation, and wsl installation for Docker to work.

Features

	Client Management: Create, read, update, and delete clients.
	Invoice Management: Create, read, update, and delete invoices, with balance calculations.
	Payment Management: Add, retrieve, update, and delete payments linked to invoices.
	User Authentication: Register and login users with JWT-based authentication.
	API Versioning: Endpoints are versioned under /api/v1/.
	SQLite Database: Lightweight, file-based database for persistence.
	Unit Tests: Comprehensive tests using xUnit for services.
	Docker Support: Containerized for local and potential cloud deployment.
	Swagger UI: Interactive API documentation at /swagger.

Architecture

	The project follows a clean architecture approach with separation of concerns:
		Domain Layer (RestAPIInvoiceManagement.Domain): Defines entities (Client, Invoice, Payment, User) and interfaces (IClientRepository, IInvoiceRepository, IPaymentRepository, IUserRepository).
		Application Layer (RestAPIInvoiceManagement.Application): Contains business logic in services (ClientService, InvoiceService, PaymentService, AuthenticationService), DTOs, and validators using FluentValidation.
		Infrastructure Layer (RestAPIInvoiceManagement.Infrastructure): Implements database access using Entity Framework Core with SQLite (AppDbContext) and repository patterns.
		API Layer (RestAPIInvoiceManagement.API): Exposes RESTful endpoints using ASP.NET Core, with controllers (ClientsController, InvoicesController, PaymentsController, UsersController), JWT authentication, and Swagger integration.
		Test Layer (RestAPIInvoiceManagement.Tests): Includes xUnit tests for services using Moq and an in-memory database.

Project Structure

	RestAPIInvoiceManagement/
	├── RestAPIInvoiceManagement.API/
	│   ├── Controllers/
	│   │   ├── ClientsController.cs
	│   │   ├── InvoicesController.cs
	│   │   ├── PaymentsController.cs
	│   │   ├── UsersController.cs
	│   ├── Middleware/
	│   │   ├── ExceptionMiddleware.cs
	│   ├── Program.cs
	│   ├── appsettings.json
	├── RestAPIInvoiceManagement.Application/
	│   ├── DTOs/
	│   │   ├── ClientDto.cs
	│   │   ├── InvoiceDto.cs
	│   │   ├── PaymentDto.cs
	│   │   ├── LoginDto.cs
	│   │   ├── RegisterDto.cs
	│   ├── Mappings/
	│   │   ├── MappingProfile.cs
	│   ├── Services/
	│   │   ├── ClientService.cs
	│   │   ├── InvoiceService.cs
	│   │   ├── PaymentService.cs
	│   │   ├── AuthenticationService.cs
	│   ├── Validators/
	│   │   ├── ClientValidator.cs
	│   │   ├── InvoiceValidator.cs
	│   │   ├── PaymentValidator.cs
	│   │   ├── LoginValidator.cs
	├── RestAPIInvoiceManagement.Domain/
	│   ├── Entities/
	│   │   ├── BaseEntity.cs
	│   │   ├── Client.cs
	│   │   ├── Invoice.cs
	│   │   ├── Payment.cs
	│   │   ├── User.cs
	│   ├── Interfaces/
	│   │   ├── IClientRepository.cs
	│   │   ├── IInvoiceRepository.cs
	│   │   ├── IPaymentRepository.cs
	│   │   ├── IUserRepository.cs
	├── RestAPIInvoiceManagement.Infrastructure/
	│   ├── Data/
	│   │   ├── AppDbContext.cs
	│   ├── Repositories/
	│   │   ├── ClientRepository.cs
	│   │   ├── InvoiceRepository.cs
	│   │   ├── PaymentRepository.cs
	│   │   ├── UserRepository.cs
	├── RestAPIInvoiceManagement.Tests/	
	│   ├── AuthenticationServiceTests.cs
	│   ├── ClientServiceTests.cs
	│   ├── InvoiceServiceTests.cs
	│   ├── PaymentServiceTests.cs
	├── Dockerfile
	├── README.md

API Structure

	All endpoints are versioned under /api/v1/. Secured endpoints require a JWT token obtained via POST /api/v1/users/login.

Endpoints

	Method
	Endpoint
	Description
	Authentication
	
	POST
	/api/v1/users
	Register a new user
	None
	
	POST
	/api/v1/users/login
	Login and obtain JWT token
	None
	
	POST
	/api/v1/clients
	Create a new client
	JWT
	
	GET
	/api/v1/clients
	List all clients (paginated)
	JWT
	
	GET
	/api/v1/clients/{id}
	Get client by ID
	JWT
	
	PUT
	/api/v1/clients/{id}
	Update client by ID
	JWT
	
	DELETE
	/api/v1/clients/{id}
	Soft delete client by ID
	JWT
	
	POST
	/api/v1/invoices
	Create a new invoice
	JWT
	
	GET
	/api/v1/invoices
	List all invoices (paginated)
	JWT
	
	GET
	/api/v1/invoices/{id}
	Get invoice by ID
	JWT
	
	PUT
	/api/v1/invoices/{id}
	Update invoice by ID
	JWT
	
	DELETE
	/api/v1/invoices/{id}
	Soft delete invoice by ID
	JWT
	
	GET
	/api/v1/invoices/{id}/balance
	Get balance due for an invoice
	JWT
	
	POST
	/api/v1/payments
	Create a new payment
	JWT
	
	GET
	/api/v1/payments/{id}
	Get payment by ID
	JWT
	
	GET
	/api/v1/payments/by-invoice/{invoiceId}
	List payments for an invoice (paginated)
	JWT
	
	PUT
	/api/v1/payments/{id}
	Update payment by ID
	JWT
	
	DELETE
	/api/v1/payments/{id}
	Soft delete payment by ID
	JWT

Example Requests

	Register User:
	POST /api/v1/users
	Content-Type: application/json
	{
	  "username": "testuser",
	  "password": "Test123!@"
	}
	Response: { "Message": "User registered successfully" }

	Login:
	POST /api/v1/users/login
	Content-Type: application/json
	{
	  "username": "testuser",
	  "password": "Test123!@"
	}
	Response: { "Token": "eyJ..." }

	Create Client:
	POST /api/v1/clients
	Authorization: Bearer <token>
	Content-Type: application/json
	{
	  "name": "Homer Simpson",
	  "email": "hs@example.com",
	  "address": "Evergreen Terrace 742"
	}

Prerequisites

	.NET SDK: Version 9.0
	Docker: Docker Desktop for Windows/Mac/Linux
	SQLite: For local database (handled automatically via EF Core)
	Command Line Tools: PowerShell, Command Prompt, or terminal
	IDE: Visual Studio, VS Code, or similar (optional)

Setup Instructions

Launch Locally

	Clone Repository:
	git clone <repository-url>
	cd archie-bolden-rest-api-invoice-management
		
	Restore Dependencies:
	dotnet restore
	
	Create SQLite Database:
	cd RestAPIInvoiceManagement.Infrastructure
	dotnet ef database update --startup-project ../RestAPIInvoiceManagement.API	
	Database created at: C:/app/InvoiceDb.db
	
	Run API:
	cd ../RestAPIInvoiceManagement.API
	dotnet run
	
	Access Swagger:
	Open http://localhost:<port>/swagger (port typically 5001 or 7165).
	Register a user, login, and use the JWT token to test secured endpoints.

Launch with Docker

	Ensure Docker is Installed:
	docker --version
	
	Recreate SQLite Database:
	cd RestAPIInvoiceManagement.Infrastructure
	dotnet ef database update --startup-project ../RestAPIInvoiceManagement.API
	
	Build Docker Image:
	cd archie-bolden-rest-api-invoice-management
	docker build -t rest-api-invoice-management .
		
	Run Docker Container:
	docker run -d -p 8080:8080 --name invoice-api rest-api-invoice-management

	Access Swagger:
	Open http://localhost:8080/swagger.
	Test endpoints as described above.

Run Unit Tests

	Navigate to Test Project:
	cd RestAPIInvoiceManagement.Tests
	
	Run Tests:
	dotnet test
	
	Check Coverage:
	dotnet test --collect:"XPlat Code Coverage"

Dependencies

	.NET 9.0: Core framework
	Microsoft.EntityFrameworkCore.Sqlite: SQLite database provider
	Swashbuckle.AspNetCore: Swagger UI for API documentation
	Microsoft.AspNetCore.Authentication.JwtBearer: JWT authentication
	AutoMapper.Extensions.Microsoft.DependencyInjection: DTO mapping
	FluentValidation.AspNetCore: Input validation
	xUnit, Moq, Microsoft.EntityFrameworkCore.InMemory: Unit testing
	Microsoft.AspNetCore.Mvc.Versioning: API versioning

Thanks for visiting the repo.


