# GymSystem

Gym management back end: members, memberships, trainers, gym classes and class booking, attendance, and online payments through Fawaterk with webhook handling.

## Features

- Auth: register, login, JWT access tokens with refresh tokens and revocation, role management (ASP.NET Core Identity)
- Members: profiles, memberships, attendance
- Classes: gym classes, trainers, class booking
- Payments: Fawaterk payment initiation, response handling and webhook DTOs
- Validation with FluentValidation, global exception handler

## Architecture

Layered solution (`GymSystem.Api` + `src/Gym.Application`, `src/Gym.Domain`, infrastructure behind `IAppDbContext`), dependency injection per layer, mappers per feature.

## Tech

.NET 9, ASP.NET Core, Entity Framework Core, SQL Server, ASP.NET Core Identity + JWT bearer, FluentValidation, Fawaterk API

## Running locally

1. Prerequisites: .NET 9 SDK, SQL Server, a Fawaterk account.
2. Set the connection string, JWT options and the Fawaterk API key with `dotnet user-secrets` (never commit them).
3. `dotnet ef database update`, then run the API. Sample requests are in `GymSystem.Api.http`.
