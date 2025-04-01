using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETL.Domain.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class SourceTypeAttribute : Attribute
{
    public string Name { get; }

    public SourceTypeAttribute(string name)
    {
        Name = name.ToLowerInvariant();
    }
}
