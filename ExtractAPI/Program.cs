using ExtractAPI.DataSources;
using ExtractAPI.Kafka;
using ExtractAPI.Services;

var builder = WebApplication.CreateBuilder(args);

var baseUrl = "https://localhost:7027";

// Register services
builder.Services.AddSingleton<IKafkaProducer, KafkaProducer>();
builder.Services.AddHttpClient<ApiDataSourceProvider>();
builder.Services.AddSingleton<DataSourceFactory>();

// Register ConfigService with factory to inject baseUrl
builder.Services.AddScoped<IConfigService>(_ =>
    new ConfigService(baseUrl));

// Register ExtractService 
builder.Services.AddScoped<IExtractService, ExtractService>();

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
