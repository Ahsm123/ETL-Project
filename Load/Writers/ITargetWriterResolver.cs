using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Load.Writers;

public interface ITargetWriterResolver
{
    ITargetWriter? Resolve(Type targetInfoType, IServiceProvider services);
}

