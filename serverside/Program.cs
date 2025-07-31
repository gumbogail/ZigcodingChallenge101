using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using serverside.Data.interfaces;
using serverside.Data.Repositories;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Movie API",
        Version = "v1",
        Description = "API for accessing TMDB movie data",
        Contact = new OpenApiContact { Name = "Your Name", Email = "your.email@example.com" }
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

// Add HttpClient with best practices
builder.Services.AddHttpClient<IMovieRepository, MovieRepository>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["TmdbApiSettings:BaseUrl"]);
    client.DefaultRequestHeaders.Accept.Clear();
    client.DefaultRequestHeaders.Accept.Add(
        new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Register repository with DI
builder.Services.AddScoped<IMovieRepository, MovieRepository>();

// Add health checks
builder.Services.AddHealthChecks();

// Configure CORS - Updated for React frontend
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins(
                "http://localhost:3000", // React development server
                "https://localhost:3000", // React with HTTPS
                "http://your-production-domain.com", // Production
                "https://your-production-domain.com") // Production with HTTPS
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });

    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Movie API V1");
        c.RoutePrefix = "api-docs"; // Access at /api-docs
        c.ConfigObject.DisplayRequestDuration = true;
    });

    // Use more permissive CORS in development
    app.UseCors("AllowAll");
}
else
{
    
    app.UseCors();

    // Enable HTTPS redirection in production
    app.UseHttpsRedirection();
}

// Add logging middleware
app.Use(async (context, next) =>
{
    Console.WriteLine($"Request: {context.Request.Method} {context.Request.Path}");
    await next();
    Console.WriteLine($"Response: {context.Response.StatusCode}");
});

// Add global error handling middleware
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync("{\"error\":\"An unexpected error occurred. Please try again later.\"}");
    });
});

app.UseRouting();
app.UseAuthorization();

// API endpoints
app.MapControllers();

// Health check endpoint
app.MapHealthChecks("/health");

// Root redirect to Swagger
app.MapGet("/", context =>
{
    context.Response.Redirect("/api-docs");
    return Task.CompletedTask;
});

// Fallback error endpoint
app.MapGet("/error", () => Results.Problem("An error occurred", statusCode: 500));

// Ensure all API routes return JSON
app.Map("/api/{**catch-all}", () => Results.NotFound(new { error = "Endpoint not found" }));

app.Run();

// Make the implicit Program class public for testing
public partial class Program { }