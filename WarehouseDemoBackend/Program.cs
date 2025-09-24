using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using WarehouseDemoBackend.Data;
using WarehouseDemoBackend.Hubs;
using WarehouseDemoBackend.Services;
using FastEndpoints.Swagger;

var builder = WebApplication.CreateBuilder(args);

// Add PostgreSQL DB
builder.Services.AddDbContext<AppDbContext>(opts =>
    opts.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

// Add SignalR
builder.Services.AddSignalR();

// Add FastEndpoints
builder.Services.AddFastEndpoints();
builder.Services.SwaggerDocument();

// Add CORS (optional, for JS clients)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins("https://localhost:7131") // MUST match frontend exactly
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); // THIS is the key!
    });
});

// Register custom services (DI)
builder.Services.AddScoped<ICoordinateService, CoordinateService>();

var app = builder.Build();

app.Use(async (context, next) =>
{
    Console.WriteLine($"➡️ Incoming: {context.Request.Method} {context.Request.Path}");
    await next();
});

app.UseCors("AllowFrontend");
app.UseRouting();

app.UseFastEndpoints();
app.UseSwaggerGen();
app.MapHub<CoordsHub>("/coordshub");

app.Run();
