# Job-Application-Tracker-API

ASP.NET Core Web API project for tracking job applications, interview steps, and notes with a layered architecture:

- `JobApplicationTracker.API`
- `JobApplicationTracker.Application`
- `JobApplicationTracker.Domain`
- `JobApplicationTracker.Persistence`

## Features

- JWT-based register and login
- Job application CRUD
- Application status tracking with enums
- Per-application notes
- Interview scheduling and result tracking
- EF Core with SQL Server

## Run

1. Update the connection string in `JobApplicationTracker.API/appsettings.json`
2. Run the database migration:

```bash
dotnet ef database update --project JobApplicationTracker.Persistence --startup-project JobApplicationTracker.API
```

3. Start the API:

```bash
dotnet run --project JobApplicationTracker.API
```
