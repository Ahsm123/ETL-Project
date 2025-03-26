using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETL.Domain.Model.SourceInfo
{
    public class DbSourceBaseInfo : SourceInfoBase
    {
        [Required(ErrorMessage = "Connection string er påkrævet.")]
        public string ConnectionString { get; set; }
        public string TableName { get; set; }
        public string Query {  get; set; }
        public string Provider { get; set; }

    }
}
