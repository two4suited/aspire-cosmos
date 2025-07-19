# Copilot Instructions for aspire-cosmos

**This is a .NET Aspire project.**

##

**DO** Use Aspire CLI to run project `aspire run`
**DO** Use this as the documentation page for Aspire.  https://learn.microsoft.com/en-us/dotnet/aspire/

## Project Overview
- **aspire-cosmos** is a .NET Aspire solution with a modular architecture, designed for cloud-native development and Azure integration.
- The solution is organized into multiple projects under `src/`, including:
  - `aspire-cosmos.ApiService`: Main API backend (likely RESTful)
  - `aspire-cosmos.Web`: Blazor-based frontend
  - `aspire-cosmos.AppHost`: AppHost for orchestration and local development
  - `aspire-cosmos.ServiceDefaults`: Shared service configuration and extensions
- Infrastructure-as-Code is managed in the `infra/` directory (Bicep/Terraform expected).

## Documentation 
- https://learn.microsoft.com/en-us/dotnet/aspire/database/azure-cosmos-db-entity-framework-integration?tabs=dotnet-cli for Cosmos EF Core integration in Aspire projects


## Key Patterns & Conventions
- **Service Boundaries:** Each subfolder in `src/` is a separate .NET project with its own concerns. Cross-service communication is explicit.
- **Configuration:** Use `appsettings.json` and `appsettings.Development.json` for environment-specific settings. Do not hardcode secrets; prefer environment variables or Azure Key Vault.
- **Dependency Injection:** All services use .NET DI. Shared extensions are in `ServiceDefaults/Extensions.cs`.
- **Frontend:** The Blazor app (`Web/`) uses Razor components in `Components/` and `Pages/`.
- **API:** The API project (`ApiService/`) is the main backend and should expose endpoints for the frontend.
- **AppHost:** Used for local orchestration and running multiple services together.

## Developer Workflows
- **Build:**
  - Use the solution file: `dotnet build src/aspire-cosmos.sln`
- **Run Locally:**
  - Use AppHost: `dotnet run --project src/aspire-cosmos.AppHost/aspire-cosmos.AppHost.csproj`
- **Add Packages:**
  - Use `dotnet add <project> package <PackageName>`
- **Infrastructure:**
  - Infra as code is in `infra/`. Use Bicep or Terraform as appropriate.

## Integration & External Dependencies
- **Azure Integration:**
  - Designed for Azure deployment (App Service, Cosmos DB, etc.).
  - Use Managed Identity for Azure SDK authentication.
- **Cosmos DB:**
  - When adding Cosmos DB, connection strings should be injected via configuration/environment, not hardcoded.

## Project-Specific Guidance
- **Shared Logic:** Place reusable code in `ServiceDefaults`.
- **Component Structure:** Blazor components are organized by feature in `Web/Components/Pages`.
- **Environment Config:** Use `appsettings.Development.json` for local overrides.
- **Secrets:** Never commit secrets. Use environment variables or Azure Key Vault.

## References
- Solution file: `src/aspire-cosmos.sln`
- Main projects: `src/aspire-cosmos.*`
- Infra: `infra/`

---

For more details, see the README or ask for architectural diagrams if needed.
