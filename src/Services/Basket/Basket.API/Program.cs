﻿var appName = "Basket API";
var builder = WebApplication.CreateBuilder();

builder.AddCustomSerilog();
builder.AddApplicationInsightsTelemetry();
builder.AddCustomSwagger();
builder.AddCustomMvc();
builder.AddCustomAuthentication();
builder.AddCustomAuthorization();
builder.AddCustomHealthChecks();
builder.AddCustomApplicationServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseCustomSwagger();
}

var pathBase = builder.Configuration["PATH_BASE"];
if (!string.IsNullOrEmpty(pathBase))
{
    app.UsePathBase(pathBase);
}

app.UseCloudEvents();

app.UseAuthentication();
app.UseAuthorization();

app.UseCors("CorsPolicy");
app.MapDefaultControllerRoute();
app.MapControllers();
app.MapSubscribeHandler();
app.MapCustomHealthChecks("/hc", "/liveness", UIResponseWriter.WriteHealthCheckUIResponse);

try
{
    app.Logger.LogInformation("Starting web host ({ApplicationName})...", appName);
    app.Run();
}
catch (Exception ex)
{
    app.Logger.LogCritical(ex, "Host terminated unexpectedly ({ApplicationName})...", appName);
}
finally
{
    Serilog.Log.CloseAndFlush();
}
