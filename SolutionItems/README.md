# Run this command to create a new migration
$env:ASPNETCORE_ENVIRONMENT = "Development"
dotnet ef migrations add {MigrationName} --project '.\04 - Infrastructure\Data\HealthyWallet.Infrastructure.Data' --startup-project '.\01 - Presentation\HealthyWallet.Web.Api'

# Run this commando to update database
dotnet ef database update --project '.\04 - Infrastructure\Data\HealthyWallet.Infrastructure.Data' --startup-project '.\01 - Presentation\HealthyWallet.Web.Api'
