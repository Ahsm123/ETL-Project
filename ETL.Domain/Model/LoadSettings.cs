using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETL.Domain.Model
{
    public class LoadSettings
    {
        [Required(ErrorMessage = "Destinationstype er påkrævet.")]
        public string DestinationType { get; set; }
        [Required(ErrorMessage = "Connection info er påkrævet.")]
        public string ConnectionInfo { get; set; }
        [Required(ErrorMessage = "Ressource er påkrævet.")]
        public string DestinationRessource { get; set; }

    }
}
