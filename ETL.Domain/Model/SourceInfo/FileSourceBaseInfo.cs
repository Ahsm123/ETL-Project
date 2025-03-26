using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETL.Domain.Model.SourceInfo
{
    public class FileSourceBaseInfo : SourceInfoBase
    {
        [Required(ErrorMessage = "Filsti er påkrævet.")]
        public string FilePath { get; set; }
        [Required(ErrorMessage = "Filtype er påkrævet.")]
        public string FileType { get; set; }
    }
}
