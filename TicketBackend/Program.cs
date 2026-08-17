using Microsoft.EntityFrameworkCore;
using TicketBackend.Data;
using Amazon;
using Amazon.SQS;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IAmazonSQS>(_ =>
    new AmazonSQSClient(RegionEndpoint.APSouth1));

// ============================================================
// Controllers
// ============================================================

builder.Services.AddControllers();

// ============================================================
// Swagger
// ============================================================

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

// ============================================================
// SQL Server / Entity Framework
// ============================================================

builder.Services.AddDbContext<TicketDbContext>(
    options =>
        options.UseSqlServer(
            builder.Configuration
                .GetConnectionString(
                    "DefaultConnection")));

// ============================================================
// CORS
// ============================================================

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "TicketFrontend",
        policy =>
        {
            policy
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

var app = builder.Build();

// ============================================================
// Swagger
// ============================================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI();
}

// ============================================================
// HTTPS
// ============================================================

app.UseHttpsRedirection();

// ============================================================
// CORS
// ============================================================

app.UseCors("TicketFrontend");

// ============================================================
// Authorization
// ============================================================

app.UseAuthorization();

// ============================================================
// Controllers
// ============================================================

app.MapControllers();

// ============================================================
// Database Initialization
// ============================================================
using (var scope = app.Services.CreateScope())
{
    var dbContext =
        scope.ServiceProvider
            .GetRequiredService<TicketDbContext>();

    await DbInitializer.InitializeAsync(
        dbContext);
}

app.Run();
