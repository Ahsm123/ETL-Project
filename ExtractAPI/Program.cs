using ETL.Domain.Sources;
using ExtractAPI.DataSources.Interfaces;
using ExtractAPI.Events;
using ExtractAPI.Events.Interfaces;
using ExtractAPI.Factories;
using ExtractAPI.Factories.Interfaces;
using ExtractAPI.Kafka;
using ExtractAPI.Kafka.Interfaces;
using ExtractAPI.Services;
using ExtractAPI.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);
var baseUrl = builder.Configuration["ConfigService:BaseUrl"];

builder.Services.Configure<KafkaSettings>(builder.Configuration.GetSection("Kafka"));

builder.Services.AddSingleton<IKafkaProducer, KafkaProducer>();
builder.Services.AddScoped<IEventDispatcher, KafkaEventDispatcher>();
builder.Services.AddScoped<DataFieldSelectorService>();
builder.Services.AddScoped<IExtractPipeline, ExtractPipeline>();

// Kafka settings
builder.Services.Configure<KafkaSettings>(
    builder.Configuration.GetSection("Kafka"));


// Register config service
builder.Services.AddHttpClient<IConfigService, ConfigService>(client =>
{
    client.BaseAddress = new Uri(baseUrl);
});

// Register all IDataSourceProvider implementations automatically
var providerTypes = typeof(IDataSourceProvider).Assembly
    .GetTypes()
    .Where(t => typeof(IDataSourceProvider).IsAssignableFrom(t) &&
                t is { IsClass: true, IsAbstract: false });

foreach (var type in providerTypes)
{
    builder.Services.AddSingleton(typeof(IDataSourceProvider), type);
}

// Register resolver
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
