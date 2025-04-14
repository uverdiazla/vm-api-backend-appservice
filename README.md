# Virtual Machine Management API

A robust RESTful API for managing virtual machines built with .NET 8 and Entity Framework Core. This project implements secure authentication with JWT tokens, role-based authorization, comprehensive error handling, well-documented API endpoints, and a relational data model.

## 🚀 Features

- **Complete VM Management**: Full CRUD operations for virtual machine resources
- **Secure Authentication**: JWT token-based authentication for API access
- **Role-Based Authorization**: Admin and Client roles with appropriate permissions
- **Swagger Documentation**: Comprehensive API documentation with SwaggerUI
- **Relational Data Model**: Properly normalized database design with relationships
- **Clean Architecture**: Following industry best practices for maintainable code
- **Error Handling**: Consistent and informative error responses

## 📋 Requirements

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [MySQL Server](https://dev.mysql.com/downloads/mysql/) (or [MariaDB](https://mariadb.org/download/))
- [Visual Studio 2022](https://visualstudio.microsoft.com/vs/) or [Visual Studio Code](https://code.visualstudio.com/) (optional)

## 🔧 Getting Started

### Installation

1. **Install .NET 8 SDK**:
   Download and install the .NET 8 SDK from [https://dotnet.microsoft.com/download/dotnet/8.0](https://dotnet.microsoft.com/download/dotnet/8.0)

2. **Clone the repository**:
   ```bash
   git clone https://github.com/yourusername/vm-api-backend-appservice.git
   cd vm-api-backend-appservice
   ```

3. **Configure the database**:
   - Install MySQL or MariaDB if not already installed
   - Create a new empty database: 
     ```sql
     CREATE DATABASE VirtualMachineDB;
     ```
   - Update connection string in `appsettings.json` if needed:
     ```json
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=VirtualMachineDB;User=root;Password=your_password;"
     }
     ```

4. **Install Entity Framework tools** (if not already installed):
   ```bash
   dotnet tool install --global dotnet-ef
   ```

5. **Apply database migrations**:
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

6. **Run the application**:
   ```bash
   dotnet run
   ```

7. **Access the Swagger UI**:
   Open your browser and navigate to:
   ```
   http://localhost:5000/
   ```
   (The port might be different depending on your configuration)

## 🏗️ Architecture

The project follows a clean architecture approach with the following components:

### Models
- **Domain Models**: Core business entities (VirtualMachine, Status, VmOperatingSystem, User)
- **DTOs**: Data Transfer Objects for API requests and responses
- **Enums**: Type-safe enumerations for statuses, OS types, and roles

### Controllers
- **AuthController**: Handles user authentication
- **VirtualMachinesController**: Manages VM CRUD operations

### Services
- **AuthService**: Implements authentication and token generation logic
- **VirtualMachineService**: Business logic for VM operations

### Repositories
- **UserRepository**: Data access for user entities
- **VirtualMachineRepository**: Data access for VM entities

### Middleware
- **ExceptionHandlingMiddleware**: Global exception handling
- **RoleValidationMiddleware**: Role-based access control

### Data Model
- **Relational Database**: Properly normalized tables with relationships
- **Seed Data**: Pre-populated operating systems, statuses, and user accounts

## 🔐 Authentication and Authorization

The API implements JWT-based authentication with role-based authorization:

- **Admin Role**: Can perform all operations (GET, POST, PUT, DELETE)
- **Client Role**: Can only view resources (GET operations)

### Login

**POST** `/api/login`

Request body:
```json
{
  "email": "admin@vmapi.com",
  "password": "Admin123!"
}
```

Response:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "name": "Admin User",
  "email": "admin@vmapi.com",
  "role": "Admin"
}
```

### Using the token

Include the JWT token in the Authorization header for protected endpoints:

```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

## 📝 API Endpoints

### Virtual Machines

| Method | Endpoint        | Description                   | Authorization    |
|--------|-----------------|-------------------------------|------------------|
| GET    | `/api/vms`      | Get all virtual machines      | Any authenticated user |
| GET    | `/api/vms/{id}` | Get a specific virtual machine| Any authenticated user |
| POST   | `/api/vms`      | Create a new virtual machine  | Admin only       |
| PUT    | `/api/vms/{id}` | Update a virtual machine      | Admin only       |
| DELETE | `/api/vms/{id}` | Delete a virtual machine      | Admin only       |

### Authentication

| Method | Endpoint        | Description                   | Authorization    |
|--------|-----------------|-------------------------------|------------------|
| POST   | `/api/login`    | Authenticate user             | Anonymous        |

## 📊 Data Model

### Virtual Machine

```json
{
  "id": 1,
  "name": "Web Server",
  "cores": 4,
  "ram": 16,
  "disk": 500,
  "operatingSystem": {
    "id": 5,
    "name": "Ubuntu 22.04 LTS",
    "version": "22.04"
  },
  "status": {
    "id": 1,
    "name": "Running",
    "colorCode": "#4CAF50"
  },
  "description": "Main web server",
  "hostname": "web-srv-01",
  "ipAddress": "10.0.0.10",
  "createdAt": "2023-01-01T00:00:00Z",
  "updatedAt": "2023-01-01T00:00:00Z"
}
```

### Operating Systems

The API comes pre-populated with common operating systems:
- Windows Server 2019/2022
- Ubuntu 18.04/20.04/22.04 LTS
- CentOS 7/8
- Debian 10/11
- And more

### Statuses

VM statuses are pre-defined with color codes for UI presentation:
- Running (Green)
- Stopped (Red)
- Provisioning (Blue)
- Failed (Orange)
- Suspended (Purple)
- Maintenance (Gray)

## 🛡️ Error Handling

The API returns consistent error responses with appropriate HTTP status codes:

| Status Code | Description                  |
|-------------|------------------------------|
| 400         | Bad Request - Invalid input data |
| 401         | Unauthorized - Authentication required |
| 403         | Forbidden - Insufficient permissions |
| 404         | Not Found - Resource not found |
| 500         | Internal Server Error - Server-side issue |

Example error response:
```json
{
  "error": "Virtual machine with ID 123 not found"
}
```

## 🔍 Validation

Input validation is implemented for all endpoints to ensure data integrity:

- Name field is required and has a maximum length
- CPU cores, RAM, and disk space have minimum and maximum values
- Operating system and status IDs must reference valid records
- Email addresses must be properly formatted

## 🧪 Testing

To run the tests (when implemented):

```bash
dotnet test
```

## 🔄 Future Enhancements

- **Real-time Updates**: Add SignalR for real-time VM status updates
- **User Management**: Implement user registration and management
- **Pagination**: Add pagination support for listing endpoints
- **Advanced Filtering**: Implement filtering of VMs by various properties
- **Health Monitoring**: Add health checks and monitoring endpoints
- **CI/CD Pipeline**: Configure automated builds and deployments

## 📚 Best Practices Followed

1. **Clean Code**: Clear naming conventions and modular design
2. **Single Responsibility Principle**: Each component has a single purpose
3. **Repository Pattern**: Data access abstraction for testability
4. **Entity Relationships**: Properly designed data model with relationships
5. **Error Handling**: Consistent error responses and logging
6. **Security**: JWT authentication and role-based authorization
7. **Documentation**: Comprehensive Swagger/OpenAPI documentation
8. **Input Validation**: Thorough validation of incoming data
9. **Dependency Injection**: Services and repositories are injected for loose coupling

## 📄 License

This project is licensed under the MIT License

## 👥 Contributors

- Uver Diaz - Initial work 