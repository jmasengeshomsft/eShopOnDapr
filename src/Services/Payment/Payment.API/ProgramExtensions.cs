// Only use in this file to avoid conflicts with Microsoft.Extensions.Logging
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Serilog;

namespace Microsoft.eShopOnDapr.Services.Payment.API;

public static class ProgramExtensions
{
    private const string AppName = "Payment API";

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
    public static void AddApplicationInsightsTelemetry(this WebApplicationBuilder builder)
    {
        builder.Services.AddApplicationInsightsTelemetry();
        builder.Services.AddApplicationInsightsKubernetesEnricher(diagnosticLogLevel: LogLevel.Information);
        builder.Services.AddSingleton<ITelemetryInitializer, MyTelemetryInitializer>();
    }

    public static void UseCustomSwagger(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{AppName} V1");
        });
    }

    public static void AddCustomHealthChecks(this WebApplicationBuilder builder) =>
        builder.Services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy())
            .AddDapr();

    public static void AddCustomApplicationServices(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<PaymentSettings>(builder.Configuration);

        builder.Services.AddScoped<IEventBus, DaprEventBus>();
        builder.Services.AddScoped<OrderStatusChangedToValidatedIntegrationEventHandler>();
    }
}

public class MyTelemetryInitializer : ITelemetryInitializer
{
    public void Initialize(ITelemetry telemetry)
    {
        if (string.IsNullOrEmpty(telemetry.Context.Cloud.RoleName))
        {
            //set custom role name here
            telemetry.Context.Cloud.RoleName = "Payment-API";
            telemetry.Context.Cloud.RoleInstance = "Payment-API";
        }
    }
}
