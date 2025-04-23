using Ignis.Models.Props;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Ignis.Models.Bindings
{
    public class HomeBindings
    {
        public FlowRate Current { get; set; } = new FlowRate();
        public FlowRate LastHour { get; set; } = new FlowRate();    
        public FlowRate Last3Hours { get; set; } = new FlowRate();
        public List<TagChartData> TagData { get; set; } = new List<TagChartData>();

        public List<SelectListItem> FirstStageTempTags { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> PressureTags { get; set; }= new List<SelectListItem>();
        public List<SelectListItem> SecondStageTempTags { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> CoolingTags { get; set; } = new List<SelectListItem>();
    }
}
