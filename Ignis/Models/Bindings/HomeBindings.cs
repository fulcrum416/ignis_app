using Ignis.Models.Props;

namespace Ignis.Models.Bindings
{
    public class HomeBindings
    {
        public FlowRate Current { get; set; } = new FlowRate();
        public FlowRate LastHour { get; set; } = new FlowRate();    
        public FlowRate Last3Hours { get; set; } = new FlowRate();
        public List<TagChartData> TagData { get; set; } = new List<TagChartData>();
    }
}
