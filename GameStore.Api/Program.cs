using GameStore.Api.EndPoints;
using GameStore.Application.Interfaces;
using GameStore.Application.Services;
using GameStore.Infrastructure.Data;
using GameStore.Infrastructure.Messaging;
using GameStore.Infrastructure.Repositories;
using Microsoft.Extensions.Caching.Distributed;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// 1. INFRASTRUCTURE SETUP
// ==========================================

// Add Redis Distributed Caching
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});


// ==========================================
// 2. DATA ACCESS (REPOSITORIES & CACHING)
// ==========================================

// --- GAMES ---
// 1. Register the "Club" (MongoDB Database)
builder.Services.AddScoped<MongoGameRepository>();

// 2. Register the "Bouncer" (Redis Cache Decorator)
builder.Services.AddScoped<IGameRepository>(provider => 
{
    var mongoRepo = provider.GetRequiredService<MongoGameRepository>();
    var cache = provider.GetRequiredService<IDistributedCache>();
    return new CachedGameRepository(mongoRepo, cache);
});

// --- GENRES ---
// 1. Register the "Club" (MongoDB Database)
builder.Services.AddScoped<MongoGenreRepository>(); 

// 2. Register the "Bouncer" (Redis Cache Decorator)
builder.Services.AddScoped<IGenreRepository>(provider => 
{
    var mongoRepo = provider.GetRequiredService<MongoGenreRepository>();
    var cache = provider.GetRequiredService<IDistributedCache>();
    return new CachedGenreRepository(mongoRepo, cache); 
});


// ==========================================
// 3. BUSINESS LOGIC (SERVICES)
// ==========================================

// Connect the application's rules to the API
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IGenreService, GenreService>();

// Hire the Postman!
builder.Services.AddScoped<IMessagePublisher, RabbitMqPublisher>();

// ==========================================
// 4. APP BUILD & MIDDLEWARE
// ==========================================
var app = builder.Build();

// Map the HTTP endpoints to the Services
app.MapGenresEndpoint();
app.MapGamesEndPoints();

// Seed initial data into MongoDB if it is empty
app.Services.SeedMongoDb();

// Start the application
app.Run();