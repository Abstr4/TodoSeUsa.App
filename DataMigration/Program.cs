using DataMigration.Data;
using DataMigration.ETL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using TodoSeUsa.Infrastructure.Data;

var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

var logFolder = Path.Combine(appData, "TodoSeUsa", "MigrationLogs");

Directory.CreateDirectory(logFolder);

var logFilePath = Path.Combine(logFolder, $"migration-{DateTimeOffset.UtcNow:yyyyMMdd-HHmmss}.log");

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override(
        "Microsoft.EntityFrameworkCore.Database.Command",
        Serilog.Events.LogEventLevel.Warning)
    .MinimumLevel.Override(
        "Microsoft.EntityFrameworkCore",
        Serilog.Events.LogEventLevel.Warning)
    .WriteTo.Console()
    .WriteTo.File(
        path: logFilePath,
        rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", false)
    .Build();

var services = new ServiceCollection();

services.AddLogging(b =>
{
    b.ClearProviders();
    b.AddSerilog();
});

services.AddDbContext<OldDbContext>(opt =>
    opt.UseSqlServer(builder.GetConnectionString("OldDb")));

services.AddDbContext<ApplicationDbContext>(opt =>
    opt.UseSqlServer(builder.GetConnectionString("NewDb")));

services.AddTransient<MigrationOrchestrator>();
services.AddTransient<MigrationValidator>();

var sp = services.BuildServiceProvider();

using var scope = sp.CreateScope();

var migrator = scope.ServiceProvider.GetRequiredService<MigrationOrchestrator>();
var migratorValidator = scope.ServiceProvider.GetRequiredService<MigrationValidator>();

var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

logger.LogInformation("Starting data migration...\n");

await migrator.RunAsync();

logger.LogInformation("Data migration completed: active providers, bills and products.\n");

logger.LogInformation("Validating migrated data...\n");

await migratorValidator.ValidateAsync();

logger.LogInformation("Data migration and validation completed successfully. Check the logs in: {LogFolder}", logFolder);
