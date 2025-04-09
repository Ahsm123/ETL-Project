using ETL.Domain.Sources;
using ExtractAPI.DataSources.DatabaseQueryBuilder;
using ExtractAPI.DataSources.DatabaseQueryBuilder.Interfaces;
using ExtractAPI.Interfaces;
using ExtractAPI.Kafka;
using ExtractAPI.Kafka.Interfaces;
using ExtractAPI.Messaging;
using ExtractAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Config base URL for ConfigService
var baseUrl = builder.Configuration["ConfigService:BaseUrl"];

// Register messaging 
builder.Services.AddSingleton<IMessagePublisher, KafkaMessagePublisher>();
builder.Services.AddScoped<IEventDispatcher, EventDispatcher>();

// Register pipeline services
builder.Services.AddScoped<IExtractPipeline, ExtractPipeline>();
builder.Services.AddScoped<DataFieldSelectorService>();

// Register config service with HttpClient
builder.Services.AddHttpClient<IConfigService, ConfigService>(client =>
{
    client.BaseAddress = new Uri(baseUrl);
});

// Automatically register all IDataSourceProvider implementations
var providerTypes = typeof(IDataSourceProvider).Assembly
    .GetTypes()
    .Where(t => typeof(IDataSourceProvider).IsAssignableFrom(t) &&
                t is { IsClass: true, IsAbstract: false });

foreach (var type in providerTypes)
{
    builder.Services.AddSingleton(typeof(IDataSourceProvider), type);
}

builder.Services.AddSingleton<ISqlQueryBuilder, MySQLQueryBuilder>();

// Register the provider resolver
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
