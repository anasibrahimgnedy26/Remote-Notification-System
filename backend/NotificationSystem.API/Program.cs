using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using NotificationSystem.API.Middleware;
using NotificationSystem.Business.Interfaces;
using NotificationSystem.Business.Mappings;
using NotificationSystem.Business.Services;
using NotificationSystem.Core.Interfaces;
using NotificationSystem.Data.Context;
using NotificationSystem.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// Configure Services
// ==========================================

// Database Context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories (Dependency Injection)
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IDeviceRepository, DeviceRepository>();

// Services (Dependency Injection)
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IDeviceService, DeviceService>();

// Firebase Service with HttpClient
builder.Services.AddHttpClient<IFirebaseService, FirebaseService>();

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Controllers
builder.Services.AddControllers();

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Notification System API",
        Version = "v1",
        Description = "Remote Push Notification System API with Firebase Cloud Messaging",
        Contact = new OpenApiContact
        {
            Name = "Admin",
            Email = "admin@notificationsystem.com"
        }
    });
});

// CORS - Allow Angular Admin Panel
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// ==========================================
// Configure Middleware Pipeline
// ==========================================

// Global Exception Handling Middleware
app.UseMiddleware<GlobalExceptionMiddleware>();

// Swagger (available in all environments for demo)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Notification System API V1");
    c.RoutePrefix = string.Empty; // Access Swagger at root
});

// app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

// Auto-create database on startup (for development)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.EnsureCreated();
}

app.Run();
