using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ignis.Data.DbModel
{
    public class AppLog
    {
        [Key]
        [Column(Order =0)]
        public int Id { get; set; }
        public DateTime TimeStamp { get; set; }

        [Required]
        [StringLength(128)]
        public string? Level { get; set; }

        public string? Message { get; set; }

        public string? Message_Template { get; set; }

        public string? Exception { get; set; }

        public string? Properties { get; set; }

        public string? Log_Event { get; set; }

    }
}
