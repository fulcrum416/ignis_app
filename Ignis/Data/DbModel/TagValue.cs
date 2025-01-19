using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ignis.Data.DbModel
{
    public class TagValue
    {
        [Key]
        public int Id { get; set; }

        public int TagId { get; set; }
        public DateTime? Timestamp { get; set; }
        public decimal? Value { get; set; }
        public string? Quality { get; set; }



    }
}
