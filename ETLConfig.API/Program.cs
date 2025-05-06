using ETL.Domain.JsonHelpers;
using ETLConfig.API.Infrastructure.Persistence;
using ETLConfig.API.Models.Domain;
using ETLConfig.API.Services;
using ETLConfig.API.Services.Interfaces;
using ETLConfig.API.Services.Validators;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Load configuration (MongoDb)
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));
builder.Services.AddSingleton(sp =>
    sp.GetRequiredService<IOptions<MongoDbSettings>>().Value);

// MongoDB context & repositories
builder.Services.AddSingleton<MongoConfigContext>();
builder.Services.AddSingleton<IConfigRepository, MongoConfigRepository>();

// Services
builder.Services.AddSingleton<IConfigProcessingService, ConfigProcessingService>();
builder.Services.AddSingleton<IJsonService, JsonService>();

// Validators
builder.Services.AddSingleton<IConnectionValidator, MySqlServerConnectionValidator>();
builder.Services.AddSingleton<IConnectionValidator, MsSqlConnectionValidator>();
builder.Services.AddSingleton<IConnectionValidatorResolver, ConnectionValidatorResolver>();
builder.Services.AddSingleton<IConfigValidator, ConfigValidator>();

// CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

// Controllers & Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middleware
app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();

app.MapControllers();
app.Run();
