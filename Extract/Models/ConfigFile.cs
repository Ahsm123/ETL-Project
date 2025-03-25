using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extract.Models;

public class ConfigFile
{
    public string Id { get; set; }
    public string SourceType { get; set; } // ex. "api", "file", "database"
    public string SourceInfo { get; set; } // ex. "url", "path", "connectionString"
    public string JsonContent { get; set; }
}
