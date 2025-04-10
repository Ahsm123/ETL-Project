using ETL.Domain.SQLQueryBuilder;
using ETL.Domain.SQLQueryBuilder.Interfaces;
using ExtractAPI.DataSources.DatabaseQueryBuilder;
using ExtractAPI.DataSources.DatabaseQueryBuilder.Interfaces;
using ExtractAPI.Interfaces;
using ExtractAPI.Kafka.Interfaces;
using ExtractAPI.Messaging;
using ExtractAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Base URL for config service
var baseUrl = builder.Configuration["ConfigService:BaseUrl"];

// Kafka & Event system
builder.Services.AddSingleton<IMessagePublisher, KafkaMessagePublisher>();
builder.Services.AddScoped<IEventDispatcher, EventDispatcher>();

// Pipeline
builder.Services.AddScoped<IExtractPipeline, ExtractPipeline>();
builder.Services.AddScoped<DataFieldSelectorService>();

// SqlExecutors
builder.Services.AddScoped<ISqlExecutor, MySqlExecutor>();
builder.Services.AddScoped<ISqlExecutor, MsSqlExecutor>();

// Query builders
builder.Services.AddScoped<ISqlQueryBuilder, MySQLQueryBuilder>();
builder.Services.AddScoped<ISqlQueryBuilder, MsSqlQueryBuilder>();

// ConfigService HTTP client
builder.Services.AddHttpClient<IConfigService, ConfigService>(client =>
{
    client.BaseAddress = new Uri(baseUrl);
});

// Automatically register all IDataSourceProviders
var providerTypes = typeof(IDataSourceProvider).Assembly
    .GetTypes()
    .Where(t => typeof(IDataSourceProvider).IsAssignableFrom(t) && t is { IsClass: true, IsAbstract: false });

foreach (var type in providerTypes)
{
    builder.Services.AddSingleton(typeof(IDataSourceProvider), type);
}

// Source provider resolver
builder.Services.AddSingleton<ISourceProviderResolver, SourceProviderResolver>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
