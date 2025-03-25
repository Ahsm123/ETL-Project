
using Extract.Services;

var baseUrl = "https://localhost:7027";
var configId = "string";

var configService = new ConfigService(baseUrl);
var config = await configService.GetByIdAsync(configId);

if (config is null)
{
    Console.WriteLine("Config not found");
    return;
}

Console.WriteLine($"Config id: {config.id}");
Console.WriteLine($"Config name: {config.name}");


