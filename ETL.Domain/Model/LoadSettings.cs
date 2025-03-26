using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETL.Domain.Model
{
    public class LoadSettings
    {
        public string DestinationType { get; set; }
        public string ConnectionInfo { get; set; }
        public string DestinationRessource { get; set; }

    }
}
