using ETL.Domain.MetaDataModels;
using ETL.Domain.Targets.DbTargets;
using Load.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Load.Services.DatabaseValidation
{
    public class HttpDatabaseMetadataService : IDataBaseMetadataService
    {
        private readonly HttpClient _httpClient;

        public HttpDatabaseMetadataService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<DatabaseMetaData?> FetchAsync(DbTargetInfoBase targetInfo, string type)
        {
            var requestPayload = new
            {
                targetInfo.ConnectionString,
                Type = type
            };

            var response = await _httpClient.PostAsync(
                "https://localhost:7027/api/Pipeline/metadata",
                new StringContent(JsonSerializer.Serialize(requestPayload), Encoding.UTF8, "application/json"));

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Metadata fetch failed ({type}): {error}");
            }

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<DatabaseMetaData>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
    }
}

