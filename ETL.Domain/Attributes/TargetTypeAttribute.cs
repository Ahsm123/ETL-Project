using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETL.Domain.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class TargetTypeAttribute : Attribute
{
    public string Name { get; private set; }

    public TargetTypeAttribute(string name)
    {
        Name = name.ToLowerInvariant();
    }
}
