using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETL.Domain.Model
{
    public class FieldMapping
    {
        [Required(ErrorMessage = "Kildefelt er påkrævet.")]
        public string SourceField {  get; set; }
        [Required(ErrorMessage = "Destinationsfelt er påkrævet.")]
        public string TargetField { get; set; }
    }
}
