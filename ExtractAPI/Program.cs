using ETL.Domain.Sources;
using ExtractAPI.DataSources;
using ExtractAPI.Events;
using ExtractAPI.Kafka;
using ExtractAPI.Services;

var builder = WebApplication.CreateBuilder(args);

var baseUrl = builder.Configuration["ConfigService:BaseUrl"];

builder.Services.AddSingleton<IKafkaProducer, KafkaProducer>();
builder.Services.AddScoped<IEventDispatcher, KafkaEventDispatcher>();
builder.Services.AddScoped<DataFieldSelectorService>();
builder.Services.AddScoped<IDataExtractionService, DataExtractionService>();

builder.Services.AddHttpClient<IConfigService, ConfigService>(client =>
{
    client.BaseAddress = new Uri(baseUrl);
});

builder.Services.AddHttpClient<RestApiSourceProvider>();
builder.Services.AddSingleton<RestApiSourceProvider>();
builder.Services.AddSingleton<ExcelDataSourceProvider>();

// Register factory
builder.Services.AddSingleton(provider =>
{
    Func<SourceInfoBase, IDataSourceProvider> factory = sourceInfo =>
    {
        return sourceInfo switch
        {
            RestApiSourceInfo => provider.GetRequiredService<RestApiSourceProvider>(),
            ExcelSourceInfo => provider.GetRequiredService<ExcelDataSourceProvider>(),
            _ => throw new NotSupportedException($"Unsupported source type: {sourceInfo.GetType().Name}")
        };
    };

    return factory;
});



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
