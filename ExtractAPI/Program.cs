using ExtractAPI.DataSources;
using ExtractAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<ApiDataSourceProvider>();
builder.Services.AddSingleton<DataSourceFactory>();

builder.Services.AddScoped<IConfigService, ConfigService>();
builder.Services.AddScoped<IExtractService, ExtractService>();

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
