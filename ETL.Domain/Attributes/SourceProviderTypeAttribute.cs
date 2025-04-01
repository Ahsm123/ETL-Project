using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETL.Domain.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class SourceProviderTypeAttribute : Attribute
{
    public string Name { get; }

    public SourceProviderTypeAttribute(string name)
    {
        Name = name.ToLowerInvariant();
    }
}
