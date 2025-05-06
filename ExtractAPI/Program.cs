using ETL.Domain.JsonHelpers;
using ETL.Domain.SQLQueryBuilder;
using ETL.Domain.SQLQueryBuilder.Interfaces;
using ExtractAPI.DataSources.DatabaseQueryBuilder;
using ExtractAPI.Messaging;
using ExtractAPI.Messaging.Interfaces;
using ExtractAPI.Messaging.Kafka.KafkaConfig;
using ExtractAPI.Services;
using ExtractAPI.Services.Interfaces;
using ExtractAPI.SourceProviders.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<KafkaSettings>(builder.Configuration.GetSection("Kafka"));
builder.Services.Configure<EventRoutingOptions>(builder.Configuration.GetSection("EventRouting"));

builder.Services.AddSingleton<IMessagePublisher, KafkaProducer>();
builder.Services.AddScoped<IEventRouter, EventRouter>();

builder.Services.AddScoped<IExtractPipeline, ExtractPipeline>();
builder.Services.AddScoped<IDataFieldSelectorService, DataFieldSelectorService>();
builder.Services.AddSingleton<IJsonService, JsonService>();

builder.Services.AddSingleton<IMySqlExecutor, MySqlExecutor>();
builder.Services.AddSingleton<IMsSqlExecutor, MsSqlExecutor>();

builder.Services.AddSingleton<IMySqlQueryBuilder, MySQLQueryBuilder>();
builder.Services.AddSingleton<IMsSqlQueryBuilder, MsSqlQueryBuilder>();

var baseUrl = builder.Configuration["ConfigService:BaseUrl"];
builder.Services.AddHttpClient<IConfigService, ConfigService>(client =>
{
    client.BaseAddress = new Uri(baseUrl);
});

var providerTypes = typeof(IDataSourceProvider).Assembly
    .GetTypes()
    .Where(t => typeof(IDataSourceProvider).IsAssignableFrom(t) && t is { IsClass: true, IsAbstract: false });

foreach (var type in providerTypes)
{
    builder.Services.AddSingleton(typeof(IDataSourceProvider), type);
}

builder.Services.AddSingleton<ISourceProviderResolver, SourceProviderResolver>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();
app.Run();