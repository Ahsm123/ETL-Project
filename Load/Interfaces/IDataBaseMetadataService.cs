using ETL.Domain.MetaDataModels;
using ETL.Domain.Targets.DbTargets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Load.Interfaces
{
    public interface IDataBaseMetadataService
    {
        Task<DatabaseMetaData> FetchAsync(DbTargetInfoBase targetinfo, string type);
    }
}
