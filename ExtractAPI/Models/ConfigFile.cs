namespace ExtractAPI.Models
{
    public class ConfigFile
    {
        public string Id { get; set; }
        public string SourceType { get; set; } // ex. "api", "file", "database"
        public string SourceInfo { get; set; } // ex. "url", "path", "connectionString"
        public string JsonContent { get; set; }
    }
}
