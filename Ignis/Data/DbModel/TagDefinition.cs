using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ignis.Data.DbModel
{
    public class TagDefinition
    {
        public int Id { get; set; }
        public string? UnitTag { get; set; }
        public string? DataTag { get; set; }
        public string? Description { get; set; }
        public string? SystemTag { get; set; }
        public string? Unit { get; set; }
        public string? UnitType { get;set; }
        public string? Category { get; set; }
        public string? InDate { get; set; }

    }
}
