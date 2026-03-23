using Aspire.Flowise.Client;
using Aspire.MailKit.Client;
using Flowise.Client;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using Qdrant.Client;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Detect which services are available by checking for connection strings.
bool hasQdrant = !string.IsNullOrEmpty(builder.Configuration.GetConnectionString("qdrant"));
bool hasMailDev = !string.IsNullOrEmpty(builder.Configuration.GetConnectionString("maildev"));
bool hasFlowise = !string.IsNullOrEmpty(builder.Configuration.GetConnectionString("flowise"));

if (hasQdrant)
    builder.AddQdrantClient("qdrant");

if (hasMailDev)
{
    builder.AddMailKitClient("maildev");

    builder.Services.AddTransient(sp =>
    {
        var smtpUri = new Uri(builder.Configuration.GetConnectionString("maildev")!);
        return new System.Net.Mail.SmtpClient(smtpUri.Host, smtpUri.Port);
    });
}

if (hasFlowise)
    builder.AddFlowiseClient("flowise");

builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

// --- Weather forecast (always available) ---

string[] summaries = ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast(
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        )).ToArray();
    return forecast;
}).WithName("GetWeatherForecast");

// --- Qdrant endpoints ---

if (hasQdrant)
{
    app.MapGet("/qdrant/collections", async (QdrantClient client, CancellationToken ct)
        => await client.ListCollectionsAsync(ct));
}
else
{
    app.MapGet("/qdrant/collections", () => Results.Json(
        new { error = "Qdrant Service Disabled in appsettings.json" }, statusCode: 503));
}

// --- Email/MailDev endpoints ---

if (hasMailDev)
{
    app.MapGet("/email/test", async (System.Net.Mail.SmtpClient client, CancellationToken ct)
        => await client.SendMailAsync("no-reply@example.com", "john.doe@example.com", "Test email", "This is some test content for the email.", ct));

    app.MapPost("/subscribe", async (MailKitClientFactory factory, string email) =>
    {
        ISmtpClient client = await factory.GetSmtpClientAsync();
        using var message = new System.Net.Mail.MailMessage("newsletter@yourcompany.com", email)
        {
            Subject = "Welcome to our newsletter!",
            Body = "Thank you for subscribing to our newsletter!"
        };
        await client.SendAsync(MimeMessage.CreateFromMailMessage(message));
    });

    app.MapGet("/subscribe/{*email}", async (MailKitClientFactory factory, [FromRoute] string email) =>
    {
        ISmtpClient client = await factory.GetSmtpClientAsync();
        using var message = new System.Net.Mail.MailMessage("newsletter@yourcompany.com", email)
        {
            Subject = "Welcome to our newsletter!",
            Body = "Thank you for subscribing to our newsletter!"
        };
        await client.SendAsync(MimeMessage.CreateFromMailMessage(message));
    });

    app.MapPost("/unsubscribe", async (MailKitClientFactory factory, string email) =>
    {
        ISmtpClient client = await factory.GetSmtpClientAsync();
        using var message = new System.Net.Mail.MailMessage("newsletter@yourcompany.com", email)
        {
            Subject = "You are unsubscribed from our newsletter!",
            Body = "Sorry to see you go. We hope you will come back soon!"
        };
        await client.SendAsync(MimeMessage.CreateFromMailMessage(message));
    });
}
else
{
    app.MapGet("/email/test", () => Results.Json(
        new { error = "MailDev Service Disabled in appsettings.json" }, statusCode: 503));
    app.MapPost("/subscribe", () => Results.Json(
        new { error = "MailDev Service Disabled in appsettings.json" }, statusCode: 503));
    app.MapGet("/subscribe/{*email}", () => Results.Json(
        new { error = "MailDev Service Disabled in appsettings.json" }, statusCode: 503));
    app.MapPost("/unsubscribe", () => Results.Json(
        new { error = "MailDev Service Disabled in appsettings.json" }, statusCode: 503));
}

// --- Flowise endpoints ---

if (hasFlowise)
{
    app.MapGet("/flowise/chatflows", async ([FromServices] IFlowiseClient client, CancellationToken ct)
        => await client.GetChatflowsAsync(ct));
}
else
{
    app.MapGet("/flowise/chatflows", () => Results.Json(
        new { error = "Flowise Service Disabled in appsettings.json" }, statusCode: 503));
}

app.MapDefaultEndpoints();

await app.RunAsync();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
