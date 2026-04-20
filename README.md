# Job-Application-Tracker-API

Job Application Tracker API is a backend system designed to help users manage and track job applications, interviews, and hiring processes.

Built with ASP.NET Core, EF Core, SQL Server, and JWT authentication.

## Features

- JWT Authentication & Authorization
- Job Application Management (CRUD)
- Status Tracking (Applied, Interview, Offer, Rejected)
- Notes & Interview Tracking
- Filtering, Searching, Sorting
- Clean Architecture (Layered)

## Endpoint Examples

```http
POST /api/jobs
GET /api/jobs
GET /api/jobs?status=interview
GET /api/jobs?search=google
GET /api/jobs?sortBy=date&descending=true
GET /api/dashboard
GET /health
```

## Architecture

- API -> Controllers
- Application -> Business Logic
- Domain -> Entities
- Persistence -> Database

## Run

1. Update the connection string in `JobApplicationTracker.API/appsettings.json`
2. Apply migrations:

```bash
dotnet ef database update --project JobApplicationTracker.Persistence --startup-project JobApplicationTracker.API
```

3. Run the API:

```bash
cd JobApplicationTracker.API
dotnet run
```
