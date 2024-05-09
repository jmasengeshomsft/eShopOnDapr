// Only use in this file to avoid conflicts with Microsoft.Extensions.Logging
using System.Text.Json;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Serilog;

namespace Microsoft.eShopOnDapr.Services.Catalog.API;

public static class ProgramExtensions
{
    private const string AppName = "Catalog API";

    public static void AddCustomConfiguration(this WebApplicationBuilder builder)
    {
        builder.Configuration.AddDaprSecretStore(
           "eshopondapr-secretstore",
           new DaprClientBuilder().Build());
    }

    public static void AddCustomSerilog(this WebApplicationBuilder builder)
    {
        var seqServerUrl = builder.Configuration["SeqServerUrl"];

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .WriteTo.Console()
            .WriteTo.Seq(seqServerUrl!)
            .Enrich.WithProperty("ApplicationName", AppName)
            .CreateLogger();

        builder.Host.UseSerilog();
    }

    public static void AddCustomSwagger(this WebApplicationBuilder builder) =>
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = $"eShopOnDapr - {AppName}", Version = "v1" });
        });

    public static void UseCustomSwagger(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{AppName} V1");
        });
    }

    public static void AddApplicationInsightsTelemetry(this WebApplicationBuilder builder)
    {
        builder.Services.AddApplicationInsightsTelemetry();
        builder.Services.AddApplicationInsightsKubernetesEnricher(diagnosticLogLevel: LogLevel.Information);
        builder.Services.AddSingleton<ITelemetryInitializer, MyTelemetryInitializer>();
    }
        

    public static void AddCustomHealthChecks(this WebApplicationBuilder builder) =>
        builder.Services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy())
            .AddDapr()
            .AddSqlServer(
                builder.Configuration["ConnectionStrings:CatalogDB"]!,
                name: "CatalogDB-check",
                tags: new [] { "catalogdb" });

    public static void AddCustomApplicationServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IEventBus, DaprEventBus>();
        builder.Services.AddScoped<OrderStatusChangedToAwaitingStockValidationIntegrationEventHandler>();
        builder.Services.AddScoped<OrderStatusChangedToPaidIntegrationEventHandler>();
    }

    public static void AddCustomDatabase(this WebApplicationBuilder builder)
    {


        builder.Services.AddDbContext<CatalogDbContext>(
            options => options.UseSqlServer(builder.Configuration["ConnectionStrings:CatalogDB"]!));
    }

    public static void ApplyDatabaseMigration(this WebApplication app)
    {
        // Apply database migration automatically. Note that this approach is not
        // recommended for production scenarios. Consider generating SQL scripts from
        // migrations instead.
        using var scope = app.Services.CreateScope();

        var retryPolicy = CreateRetryPolicy(app.Configuration, Log.Logger);
        var context = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();

        retryPolicy.Execute(context.Database.Migrate);

        //seed data using seedAIData method if there is an environment variable called DATA_FOLDER and a flag called SEED_AI_DATA
        // if (bool.TryParse(app.Configuration["SeedAIData"], out bool seedAIData) && seedAIData)
        // {
        //     SeedAIData(context, app.Configuration);
        // }

        var aiDataFolder = Environment.GetEnvironmentVariable("DATA_FOLDER");
        var fullPath = Path.Combine(app.Environment.ContentRootPath, aiDataFolder);
        SeedAIData(context, fullPath);
        
    }

    private static Policy CreateRetryPolicy(IConfiguration configuration, Serilog.ILogger logger)
    {
        // When running in an orchestrator/K8s, it will take care of restarting failed services.
        if (bool.TryParse(configuration["RetryMigrations"], out bool _))
        {
            return Policy.Handle<Exception>().
                WaitAndRetryForever(
                    sleepDurationProvider: _ => TimeSpan.FromSeconds(5),
                    onRetry: (exception, retry, _) =>
                    {
                        logger.Warning(
                            exception,
                            "Exception {ExceptionType} with message {Message} detected during database migration (retry attempt {retry}, connection {connection})",
                            exception.GetType().Name,
                            exception.Message,
                            retry,
                            configuration["ConnectionStrings:CatalogDB"]);
                    }
                );
        }

        return Policy.NoOp();
    }

    //create a method to seed the data in the database using the data from the /misc/data folder
    private static void SeedAIData(CatalogDbContext context, string path)
    {
        //read the data under the aiDataFolder/brands as json and convert it to the CatalogBrand class
        var brands = JsonSerializer.Deserialize<List<CatalogBrand>>(
            File.ReadAllText(Path.Combine(path, "brands.json")));
     
        var types = JsonSerializer.Deserialize<List<CatalogType>>(
            File.ReadAllText(Path.Combine(path, "types.json")));

        var items = JsonSerializer.Deserialize<List<CatalogItem>>(
            File.ReadAllText(Path.Combine(path, "items.json")));


        //make sure that all tables are empty  before seeding the data. Do not use truncate
        if (context.CatalogBrands.Any() || context.CatalogTypes.Any() || context.CatalogItems.Any())
        {     
            context.Database.ExecuteSqlRaw($"DELETE FROM CatalogItems");
            context.Database.ExecuteSqlRaw($"DELETE FROM CatalogTypes");
            context.Database.ExecuteSqlRaw($"DELETE FROM CatalogBrands"); 

            context.SaveChanges();
        }
        
       // context.Database.ExecuteSqlRaw("TRUNCATE TABLE CatalogBrands");
        context.CatalogBrands.AddRange(brands);

       // context.Database.ExecuteSqlRaw("TRUNCATE TABLE CatalogTypes");
        context.CatalogTypes.AddRange(types);

        //context.Database.ExecuteSqlRaw("TRUNCATE TABLE CatalogItems");
        context.CatalogItems.AddRange(items);

        context.SaveChanges();
    }
}


public class MyTelemetryInitializer : ITelemetryInitializer
{
    public void Initialize(ITelemetry telemetry)
    {
        if (string.IsNullOrEmpty(telemetry.Context.Cloud.RoleName))
        {
            //set custom role name here
            telemetry.Context.Cloud.RoleName = "Catalog-API";
            telemetry.Context.Cloud.RoleInstance = "Catalog-API";
        }
    }
}

//create a method to seed the data in the database
