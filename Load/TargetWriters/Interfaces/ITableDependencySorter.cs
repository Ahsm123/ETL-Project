using ETL.Domain.Config;
using ETL.Domain.MetaDataModels;
using ETL.Domain.Targets.DbTargets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Load.TargetWriters.Interfaces
{
    public interface ITableDependencySorter
    {
        List<TargetTableConfig> SortByDependency(List<TargetTableConfig> tableConfigs, DatabaseMetaData metaData);
    }
}
