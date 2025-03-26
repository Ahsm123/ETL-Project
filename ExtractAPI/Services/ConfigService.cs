using ETL.Domain.Model;
using ETL.Domain.Model.SourceInfo;
using System.Text.Json;

namespace ExtractAPI.Services
{
    public class ConfigService : IConfigService
    {
        private readonly HttpClient _httpClient;

        public ConfigService(string baseUrl)
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(baseUrl)
            };
        }


        // ConfigService henter config (json) fra Config API
        // og deserializerer det til et ConfigFile objekt
        public async Task<ConfigFile?> GetByIdAsync(string id)
        {
            // kalder config api for at hente config for et givent id, id er navnet på pipelinen, fx "pipeline_001"
            var response = await _httpClient.GetAsync($"/api/ConfigFile/{id}");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Failed to get config for ID {id}, Status: {response.StatusCode}");
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();

            // parser json direkte uden at bruge en klasse først
            var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            // vi får fat i SourceType så vi ved om det er et api, fil eller database
            var sourceType = root.GetProperty("SourceType").GetString();


            // ud fra hvilken type, bestemmer vi hvilken type sourceInfoBase vi skal deserialisere til
            var sourceInfoElement = root.GetProperty("SourceInfo");
            SourceInfoBase? sourceInfo = sourceType!.ToLower() switch
            {
                "api" => JsonSerializer.Deserialize<ApiSourceBaseInfo>(sourceInfoElement.GetRawText()),
                "db" => JsonSerializer.Deserialize<DbSourceBaseInfo>(sourceInfoElement.GetRawText()),
                "file" => JsonSerializer.Deserialize<FileSourceBaseInfo>(sourceInfoElement.GetRawText()),
                _ => throw new NotImplementedException($"Source type {sourceType} not implemented")
            };

            // resten af config deserialiseres til domænemodellen ConfigFile
            var config = new ConfigFile
            {
                Id = root.GetProperty("Id").GetString(),
                SourceType = sourceType,
                SourceInfo = sourceInfo,
                Extract = JsonSerializer.Deserialize<ExtractSettings>(root.GetProperty("Extract").GetRawText()),
                Transform = JsonSerializer.Deserialize<TransformSettings>(root.GetProperty("Transform").GetRawText()),
                Load = JsonSerializer.Deserialize<LoadSettings>(root.GetProperty("Load").GetRawText())
            };


            config!.SourceInfo = sourceInfo!;

            Console.WriteLine($"Id: {config.Id}");
            Console.WriteLine($"SourceType: {config.SourceType}");
            Console.WriteLine($"SourceInfo: {config.SourceInfo.GetType().Name}");

            return config;
        }
    }
}
