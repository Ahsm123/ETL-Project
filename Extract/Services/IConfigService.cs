using Extract.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extract.Services;

public interface IConfigService
{
    Task<ConfigFile?> GetByIdAsync(int id);
}
