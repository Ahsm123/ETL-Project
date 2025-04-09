using ETLConfig.API.Infrastructure.Persistence;
using ETLConfig.API.Services;
using ETLConfig.API.Services.Interfaces;
using ETLConfig.API.Services.Validators;

var builder = WebApplication.CreateBuilder(args);
DotNetEnv.Env.Load();

var mongoSettings = new
{
    Connection = Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING"),
    Database = Environment.GetEnvironmentVariable("MONGO_DATABASE_NAME"),
    Collection = Environment.GetEnvironmentVariable("MONGO_COLLECTION_NAME")
};




builder.Services.AddSingleton(new MongoConfigContext(
    mongoSettings.Connection,
    mongoSettings.Database,
    mongoSettings.Collection
));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AddSingleton<IConnectionValidator, MySqlServerConnectionValidator>();
builder.Services.AddSingleton<IConnectionValidator, MsSqlConnectionValidator>();
builder.Services.AddSingleton<IConnectionValidatorResolver, ConnectionValidatorResolver>();


builder.Services.AddSingleton<IConfigRepository, MongoConfigRepository>();
builder.Services.AddSingleton<IConfigProcessingService, ConfigProcessingService>();
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

app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();
app.Run();

