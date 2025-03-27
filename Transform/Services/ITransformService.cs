using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transform.Services
{
    public interface ITransformService
    {
        Task TransformDataAsync(string configId);
    }
}
