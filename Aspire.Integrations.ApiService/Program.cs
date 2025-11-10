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

// Add qdrant
builder.AddQdrantClient("qdrant");

// Add MailKit services to the container (using maildev connection string).
builder.AddMailKitClient("maildev");

// Add Flowise client
builder.AddFlowiseClient("flowise");

// Add SmtpClient from MailDev integration
builder.Services.AddTransient(sp =>
{
    var smtpUri = new Uri(builder.Configuration.GetConnectionString("maildev")!);

    return new System.Net.Mail.SmtpClient(smtpUri.Host, smtpUri.Port);
});

// Add services to the container.
builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

string[] summaries = ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

// qdrant example(s)
app.MapGet("/qdrant/collections", async (QdrantClient client, CancellationToken cancellationToken) => await client.ListCollectionsAsync(cancellationToken));

// maildev example(s)
app.MapGet("/email/test", async (System.Net.Mail.SmtpClient client, CancellationToken cancellationToken) => await client.SendMailAsync("no-reply@example.com", "john.doe@example.com", "Test email", "This is some test content for the email.", cancellationToken));

// flowise example(s)
app.MapGet("/flowise/chatflows", async ([FromServices] IFlowiseClient client, CancellationToken cancellationToken)
    => await client.GetChatflowsAsync(cancellationToken));

// MailKit example(s)
app.MapPost("/subscribe",
    async (MailKitClientFactory factory, string email) =>
    {
        ISmtpClient client = await factory.GetSmtpClientAsync();

        using var message = new System.Net.Mail.MailMessage("newsletter@yourcompany.com", email)
        {
            Subject = "Welcome to our newsletter!",
            Body = "Thank you for subscribing to our newsletter!"
        };

        await client.SendAsync(MimeMessage.CreateFromMailMessage(message));
    });

app.MapGet("/subscribe/{*email}",
    async (MailKitClientFactory factory, [FromRoute] string email) =>
    {
        ISmtpClient client = await factory.GetSmtpClientAsync();

        using var message = new System.Net.Mail.MailMessage("newsletter@yourcompany.com", email)
        {
            Subject = "Welcome to our newsletter!",
            Body = "Thank you for subscribing to our newsletter!"
        };

        await client.SendAsync(MimeMessage.CreateFromMailMessage(message));
    });

app.MapPost("/unsubscribe",
    async (MailKitClientFactory factory, string email) =>
    {
        ISmtpClient client = await factory.GetSmtpClientAsync();

        using var message = new System.Net.Mail.MailMessage("newsletter@yourcompany.com", email)
        {
            Subject = "You are unsubscribed from our newsletter!",
            Body = "Sorry to see you go. We hope you will come back soon!"
        };

        await client.SendAsync(MimeMessage.CreateFromMailMessage(message));
    });

app.MapDefaultEndpoints();

await app.RunAsync();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
