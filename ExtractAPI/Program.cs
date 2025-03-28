using ExtractAPI.DataSources;
using ExtractAPI.Events;
using ExtractAPI.Kafka;
using ExtractAPI.Services;

var builder = WebApplication.CreateBuilder(args);

var baseUrl = "https://localhost:7027";

// Register services
builder.Services.AddSingleton<IKafkaProducer, KafkaProducer>();
builder.Services.AddHttpClient<ApiDataSourceProvider>();
builder.Services.AddSingleton<DataSourceProviderFactory>();
builder.Services.AddScoped<IEventDispatcher, KafkaEventDispatcher>();
builder.Services.AddScoped<DataFieldSelectorService, DataFieldSelectorService>();

// Register ConfigService with factory to inject baseUrl
builder.Services.AddHttpClient<IConfigService, ConfigService>(client =>
{
    client.BaseAddress = new Uri(baseUrl);
});

// Register ExtractService 
builder.Services.AddScoped<IDataExtractionService, DataExtractionService>();

// Swagger + Controllers
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
