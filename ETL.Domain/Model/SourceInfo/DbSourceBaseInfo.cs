using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETL.Domain.Model.SourceInfo
{
    public class DbSourceBaseInfo : SourceInfoBase
    {
        public string ConnectionString { get; set; }
        public string TableName { get; set; }
        public string Query {  get; set; }
        public string Provider { get; set; }

    }
}
