# Efcore10 Project - README

Overview
--------
This repository contains a .NET 10 project that demonstrates using Entity Framework Core with SQL Server. It includes a DbContext (ProductDBContext) and controllers for working with product DTOs.

Prerequisites
-------------
- .NET 10 SDK installed: https://dotnet.microsoft.com/en-us/download
- Visual Studio 2026 (optional) with .NET workload, or any code editor + CLI
- SQL Server instance accessible (LocalDB, SQL Express, Docker, or remote server)

Repository layout (typical)
--------------------------
- Controllers/ - API controllers
- Data/ - ProductDBContext and entity configurations
- Models/ - domain models and DTOs
- Program.cs - application startup and DI registration
- appsettings.json - configuration including connection strings

Common setup steps
------------------
1. Restore packages and build

   In a terminal at the repository root:

   powershell
   dotnet restore
   dotnet build

2. Install EF Core provider and design packages (if not already installed)

   - SQL Server provider (runtime): Microsoft.EntityFrameworkCore.SqlServer
   - Design-time helpers: Microsoft.EntityFrameworkCore.Design
   - Tools for Package Manager Console (optional in VS): Microsoft.EntityFrameworkCore.Tools

   Using dotnet CLI (replace <ProjectPath> with path to the .csproj that contains ProductDBContext):

   dotnet add "<ProjectPath>" package Microsoft.EntityFrameworkCore.SqlServer
   dotnet add "<ProjectPath>" package Microsoft.EntityFrameworkCore.Design

   Optionally install the global EF CLI (dotnet-ef) if using dotnet ef commands:

   dotnet tool install --global dotnet-ef
   --or--
   dotnet tool update --global dotnet-ef

   If you prefer Visual Studio's Package Manager Console, install the tools package into the project that contains the DbContext:

   Install-Package Microsoft.EntityFrameworkCore.Tools

Configuring the connection string
---------------------------------
Add a connection string in appsettings.json in the startup project (example):

{
  "ConnectionStrings": {
	"DefaultConnection": "Server=.;Database=MyDatabase;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}

In Program.cs register the DbContext (example):

using Microsoft.EntityFrameworkCore;

builder.Services.AddDbContext<ProductDBContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

Make sure you have the using directive: using Microsoft.EntityFrameworkCore;

Creating migrations
-------------------
Two common approaches: Visual Studio Package Manager Console (PMC) or dotnet CLI.

1) Using Visual Studio Package Manager Console (recommended inside VS)

- Open Tools → NuGet Package Manager → Package Manager Console
- Set the "Default project" dropdown to the project that contains ProductDBContext
- Ensure the startup project (right-click → Set as Startup Project) is the one used to run the app
- Run:

  Install-Package Microsoft.EntityFrameworkCore.Tools   # if not installed
  Add-Migration InitialCreate
  Update-Database

2) Using dotnet CLI (PowerShell / terminal)

- Ensure dotnet-ef is installed globally (see earlier)
- From repository root (adjust paths):

  dotnet ef migrations add InitialCreate --project "./Data" --startup-project "./WebApi"
  dotnet ef database update --project "./Data" --startup-project "./WebApi"

- If your DbContext project and startup project are the same, you can omit the --project or --startup-project flags.

Troubleshooting
---------------
- "Add-Migration: The term 'Add-Migration' is not recognized":
  - You ran the command in a plain PowerShell window instead of Visual Studio PMC and didn't install dotnet-ef.
  - Solution: run Install-Package Microsoft.EntityFrameworkCore.Tools in PMC, or install dotnet-ef globally and use dotnet ef from CLI.

- "No executable found matching command 'dotnet-ef'":
  - Install or update the global tool: dotnet tool install --global dotnet-ef

- "Design-time factory or DbContext not found" when running migrations:
  - Ensure the project builds and a public DbContext-derived type exists.
  - If your Program.cs uses top-level statements, EF Core can usually discover the DbContext if it's registered in the host builder; otherwise add an IDesignTimeDbContextFactory implementation.

- "UseSqlServer" or "Microsoft.EntityFrameworkCore" not found:
  - Add using Microsoft.EntityFrameworkCore;
  - Ensure Microsoft.EntityFrameworkCore.SqlServer package is referenced in the project that calls UseSqlServer.

Running the app
---------------
From the repo root:

dotnet run --project "Path\To\StartupProject.csproj"

Or run from Visual Studio using the configured startup project.

Additional notes
----------------
- Keep project that contains migrations separate from the startup project only if you have a multi-project solution; remember to set --project and --startup-project when using dotnet ef.
- After changing model classes, create a new migration and apply it with Update-Database (PMC) or dotnet ef database update (CLI).

If you want targeted help, provide:
- Path to the project (.csproj) that contains ProductDBContext
- Which project is the startup project
- Exact errors you see when running migration commands

License
-------
This README is provided as-is for repository setup and usage guidance.
