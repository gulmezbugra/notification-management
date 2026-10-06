# Notification Management System

ASP.NET Core MVC (.NET 10) - N-Tier - Factory Pattern - EF Core - SQL Server.

## Run

1. Install the .NET 10 SDK and SQL Server LocalDB (ships with Visual Studio) or any SQL Server.
   Change `ConnectionStrings:DefaultConnection` in `NotificationManagement.Mvc/appsettings.json` if needed.
2. Install the EF tool (once):  `dotnet tool install --global dotnet-ef`
3. Create the initial migration (from the solution folder):

       dotnet ef migrations add InitialCreate --project NotificationManagement.Data --startup-project NotificationManagement.Mvc

4. Run the app (in Development it applies migrations automatically and creates `NotificationManagementDb`):

       dotnet run --project NotificationManagement.Mvc

   Or apply manually:  `dotnet ef database update --project NotificationManagement.Data --startup-project NotificationManagement.Mvc`
5. Tests: `dotnet test`

## Using .NET 8 or 9
Replace `net10.0` with `net8.0`/`net9.0` in every .csproj and change the `10.0.0` package versions to `8.0.x`/`9.0.x`.

## Try it
- Email: `user@example.com`  - SMS/WhatsApp: `+905551234567`  - Push: `device-token-123`
- Put `[fail]` in the message to simulate a provider failure (status becomes Failed).

## Factory pattern in one paragraph
Every channel class implements `INotification` and declares its own `Type`. `AddBusinessLayer()` registers all of them.
`NotificationFactory` receives `IEnumerable<INotification>`, indexes them by `Type`, and `Create(type)` is a dictionary lookup.
`NotificationService` only talks to `INotificationFactory`, so adding a channel means: add an enum value + one class. Nothing else changes.
