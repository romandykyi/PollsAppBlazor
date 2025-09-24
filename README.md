# PollsAppBlazor

A functional personal project: a simple polls web app built with ASP.NET Core and Blazor WebAssembly. Designed for general users, it allows creating polls, voting anonymously, and managing favorites.

Live demo: https//pollsappblazor.online

## Features

- Users can create polls with multiple options.
- Anonymous voting; votes cannot be changed once submitted.
- Polls can have an expiration date.
- Votes visibility options: always visible, visible only after voting, or only after the poll ends.
- Users can mark polls as favorites.
- Admin access for managing polls.
- Authentication via Duende IdentityServer.

## Tech Stack

- **Front End**: Blazor WebAssembly, MudBlazor
- **Back End**: ASP.NET Core, Entity Framework Core, Duende IdentityServer, Swashbuckle (Swagger)
- **Database**: PostgreSQL
- **Communication**: Azure Communication Services (registration and reset password mails).
- **Hosting**: Azure Web App Service (Linux), PostgreSQL database hosted on Aiven.

## Architecture

PollsAppBlazor follows a layered architecture with clear separation of concerns:
`Shared <- DataAccess <- Application <- Server -> Client -> Shared`

### Backend (3 layers)

**DataAccess:**
  - Implements repositories that interact with the database using Entity Framework Core.
  - Encapsulates LINQ queries and persistence logic.
  - Contains migrations.

**Application:**
  - Contains the business logic of the application.
  - Uses repositories from the DataAccess layer to implement services.

**Server:**
  - Exposes controllers for the API endpoints.
  - Handles authentication, authorization, and policies (via Duende IdentityServer).
  - Provides configuration and dependency injection setup.
  - Performs simple validation using built-in data annotations.

### Frontend
**Client**
  - Built with Blazor WebAssembly and styled with MudBlazor.
  - Communicates with the Server through HTTP APIs.
  - Provides the user interface for creating polls, voting, and managing favorites.

### Shared (PollsAppBlazor.Shared)

- Defines common DTOs and models used across Client, Server, and Application layers.
- Ensures conistencsy of contracts between frontend and backend.

### Local Setup

1. Ensure .NET 9+ and PostgreSQL are installed.
2. Clone the repository.
3. Add your connection string in secrets.json under ConnectionStrings:DefaultConnection:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5433;Database=pollsappblazor;Username=...;Password=...;"
}
```

4. Run database migrations:

`dotnet ef database update --startup-project PollsAppBlazor.Server --project PollsAppBlazor.DataAccess`

5. Run the app
- Frontend available at http://localhost:7093
- Swagger UI available at http://localhost:7093/swagger (development only)

**Notes:** The app automatically seeds fake data in the development environment. Emails are sent through Azure Email Communication Service but are printed to the console in development.

### Admin Access (local development only)

-- Login: ```admin```
-- Password: ```P@ssw0rd1sNotS3cur3^:(```

## Screenshots

<img width="1919" height="862" alt="screen1" src="https://github.com/user-attachments/assets/84c44db3-e48f-4439-9da2-d21b76505947" />

<img width="1919" height="862" alt="screen2" src="https://github.com/user-attachments/assets/7c81ded7-7bee-4096-bc51-93ec06d6903f" />

<img width="1914" height="860" alt="screen3" src="https://github.com/user-attachments/assets/63fa312b-a7e0-48bb-aeb8-73a096a0f061" />

<img width="1919" height="857" alt="screen4" src="https://github.com/user-attachments/assets/5e379715-a823-4d69-8163-87462187117d" />

