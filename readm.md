# Efcore10 — Detailed README (readm.md)

Purpose
-------
This file documents a practical step-by-step setup and troubleshooting guide for a .NET 10 project using Entity Framework Core with SQL Server. It is intended for developers who need exact commands and common diagnostics for migrations and database updates.

Environment & prerequisites
---------------------------
- .NET 10 SDK installed and on PATH (dotnet --version)
- A supported IDE (Visual Studio 2026 recommended) or a code editor + CLI
- SQL Server accessible (LocalDB, SQL Express, Docker container, or remote)
- Optional: Administrative rights to create databases on local SQL Server

Packages to have in the project containing your DbContext
-------------------------------------------------------
- Microsoft.EntityFrameworkCore.SqlServer  # EF Core SQL Server provider
- Microsoft.EntityFrameworkCore.Design     # Design-time components required by dotnet-ef
- Microsoft.EntityFrameworkCore.Tools      # (When using Visual Studio PMC)

Install packages (examples)
---------------------------
From repository root (PowerShell):

dotnet add "<Path\To\YourProject>.csproj" package Microsoft.EntityFrameworkCore.SqlServer
dotnet add "<Path\To\YourProject>.csproj" package Microsoft.EntityFrameworkCore.Design

If you use Package Manager Console (Visual Studio) instead:

Install-Package Microsoft.EntityFrameworkCore.SqlServer
Install-Package Microsoft.EntityFrameworkCore.Design
Install-Package Microsoft.EntityFrameworkCore.Tools  # for PMC commands

Install the EF CLI (dotnet-ef) globally if you want to use dotnet ef commands:

dotnet tool install --global dotnet-ef
--or--
dotnet tool update --global dotnet-ef

DbContext registration (Program.cs)
----------------------------------
Ensure your startup Program.cs registers the DbContext and that you have the EF Core namespace:

using Microsoft.EntityFrameworkCore;

builder.Services.AddDbContext<ProductDBContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

appsettings.json example
------------------------
Place the connection string in the startup project's appsettings.json (or use environment variables):

{
  "ConnectionStrings": {
	"DefaultConnection": "Server=localhost;Database=MyDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}

Where to run migration commands
-------------------------------
- If using Visual Studio Package Manager Console (PMC): Open Tools → NuGet Package Manager → Package Manager Console. Set the "Default project" dropdown to the project that contains ProductDBContext. Ensure the startup project (used to resolve configuration) is set in Solution Explorer.
- If using dotnet CLI: run dotnet ef commands from the repo root and supply --project and --startup-project as needed.

Common commands — Visual Studio PMC
---------------------------------
In PMC with Default project set to the project containing the DbContext:

Install-Package Microsoft.EntityFrameworkCore.Tools   # once, if needed
Add-Migration InitialCreate
Update-Database

Common commands — dotnet CLI (examples)
--------------------------------------
Assume DbContext project is Data\MyDataProject.csproj and startup project is WebApi\MyApi.csproj

dotnet ef migrations add InitialCreate --project "Data/MyDataProject.csproj" --startup-project "WebApi/MyApi.csproj"
dotnet ef database update --project "Data/MyDataProject.csproj" --startup-project "WebApi/MyApi.csproj"

If the DbContext and startup project are the same, you can omit --project or --startup-project.

Design-time DbContext discovery issues
-------------------------------------
EF tries to discover the DbContext at design time by building the startup project and reading the service registration. If EF cannot find the DbContext, options:

1) Ensure the startup project builds and the DbContext type is public
2) Provide a design-time factory by implementing IDesignTimeDbContextFactory<TContext> in the project that contains the DbContext. Example minimal factory:

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ProductDBContext>
{
	public ProductDBContext CreateDbContext(string[] args)
	{
		var optionsBuilder = new DbContextOptionsBuilder<ProductDBContext>();
		optionsBuilder.UseSqlServer("Server=localhost;Database=MyDb;Trusted_Connection=True;");
		return new ProductDBContext(optionsBuilder.Options);
	}
}

This factory is only used at design time to create the DbContext for tools.

Troubleshooting common errors
-----------------------------
- "Add-Migration: The term 'Add-Migration' is not recognized":
  - You ran a plain PowerShell instead of PMC and don't have dotnet-ef installed. Use dotnet ef or open PMC and use Add-Migration after installing EF Tools.

- "No executable found matching command 'dotnet-ef'":
  - Install the global tool: dotnet tool install --global dotnet-ef

- "Type or namespace name 'UseSqlServer' could not be found":
  - Add using Microsoft.EntityFrameworkCore; and ensure Microsoft.EntityFrameworkCore.SqlServer package is referenced in the project that calls UseSqlServer.

- "Unable to create an object of type 'ProductDBContext'":
  - Ensure your DbContext has a public constructor that accepts DbContextOptions<ProductDBContext> and the project builds.
  - Add a design-time factory if discovery fails.

- Migration compilation/build errors:
  - Fix compiler errors in the project first. Tools require a successful build to scaffold migrations.

Best practices
--------------
- Keep migrations in the same project as the DbContext unless you intentionally separate them; if separated, always use --project and --startup-project flags.
- Keep connection strings out of source control (use secrets, environment variables, or user-specific appsettings.Development.json).
- Add migrations frequently and review the generated Up/Down methods before applying to production.

If you need help
---------------
Provide:
- Path to the .csproj that contains ProductDBContext
- Which project is the startup project (the one that runs the app)
- Exact commands you ran and the full error output
