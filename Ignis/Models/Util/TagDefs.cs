using Ignis.Data;
using Ignis.Data.DbModel;
using Ignis.Models.Props;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.Data;

namespace Ignis.Models.Util
{
    public interface ITagDefs
    {
        Task<FlowRate> GetAverageFlowToLastHours(int startHour, string tagName);
        Task<FlowRate> GetAverageFlowToNow(int lastHours, string tagName);
        Task<List<SelectListItem>> GetTagCatgoriesSelectListAsync(string selectedText="");
        Task<List<TagChartData>> GetTagData(int totalHours, List<string> tagNames);
        Task<List<SelectListItem>> GetTagDefSelectList(string tagType, string category);
        Task<List<SelectListItem>> GetUnitTagsByCategorySelectListAsync(string category, string selectedCat = "");
        Task<List<SelectListItem>> GetUnitTagsByUnitTypeSelectListAsync(string unitType, string selectedTag = "");
        Task<List<SelectListItem>> GetUnitTypeSelectListAsync(string selectedText = "");
    }
    public class TagDefs:ITagDefs
    {
        private ILogger<TagDefs> _logger;
        private AppDbContext _db;

        public TagDefs(ILogger<TagDefs> logger, AppDbContext db) 
        { 
            _logger = logger;
            _db = db;
        }

        public Task<List<SelectListItem>> GetTagCatgoriesSelectListAsync(string selectedText="")
        {
            var cat = _db.TagsDefinitions.DistinctBy(m=>m.Category).ToList();
            var results = new List<SelectListItem>();
            foreach (var item in cat)
            {
                results.Add(new SelectListItem
                {
                    Text = item.Category,
                    Value = item.Category.ToString(),
                    Selected = selectedText == item.Category
                });
            }

            return Task.FromResult(results);
        }

        public Task<List<SelectListItem>> GetUnitTypeSelectListAsync(string selectedText = "")
        {
            var unitTypes = _db.TagsDefinitions.DistinctBy(m=>m.UnitType).ToList();
            var results = new List<SelectListItem>();   
            foreach(var item in unitTypes)
            {
                results.Add(new SelectListItem
                {
                    Text = item.UnitType,
                    Value = item.UnitType.ToString(),
                    Selected = selectedText==item.UnitType
                });
            }

            return Task.FromResult(results);
        }

        public Task<List<SelectListItem>> GetUnitTagsByUnitTypeSelectListAsync(string unitType, string selectedTag = "")
        {
            var tags = _db.TagsDefinitions.Where(m=>m.UnitType== unitType).ToList();
            var results = new List<SelectListItem>();
            foreach(var item in tags)
            {
                results.Add(new SelectListItem
                {
                    Text=item.UnitTag,
                    Value=item.UnitTag.ToString(),
                    Selected=item.UnitTag==selectedTag
                });
            }

            return Task.FromResult(results);
        }

        public Task<List<SelectListItem>> GetUnitTagsByCategorySelectListAsync(string category, string selectedCat = "")
        {
            var tags = _db.TagsDefinitions.Where(m=>m.Category== category).ToList();   
            var results = new List<SelectListItem>();
            foreach(var item in tags)
            {
                results.Add(new SelectListItem
                {
                    Text = item.UnitTag,
                    Value = item.UnitTag.ToString(),
                    Selected = item.UnitTag == selectedCat
                });
            }
            return Task.FromResult(results);
        }

        public Task<List<TagChartData>> GetTagData(int totalHours, List<string> tagNames)
        {
            var since = DateTime.UtcNow.AddHours(-totalHours);
            //var data = _db.Tags.Where(m => m.Name != null
            //&& tagNames.Contains(m.Name)
            //&& m.LogDate >= since
            //&& m.LogDate <= DateTime.UtcNow
            //).OrderBy(x=>x.InDate).Select(x=>new TagChartData
            //{
            //    Name = $"{x.Name} - {x.Description}",
            //    TagValue=x.TagValue.Value,
            //    Timestamp=x.InDate.ToLocalTime()
            //}).OrderBy(m=>m.Timestamp).ToList();

            // Query the database
            var data = _db.Tags
                .Where(m => m.Name != null              // Ensure Name is not null
                         && tagNames.Contains(m.Name)  // Filter by the desired tag names
                         && m.LogDate >= since         // Filter by start date/time (inclusive)
                         && m.LogDate <= DateTime.UtcNow) // Filter by end date/time (inclusive)
                                                          // Note: Ordering by InDate here might be redundant if you order by Timestamp later,
                                                          // unless needed for specific database behavior or intermediate processing.
                                                          // Consider ordering only once after the Select if Timestamp is the final desired order.
                                                          // .OrderBy(x => x.InDate) // Original OrderBy - potentially removable
                .Select(x => new TagChartData
                {
                    Name = $"{x.Name} - {x.Description}", // String interpolation handles null Description gracefully
                                                          // Use null-coalescing operator: If x.TagValue is null, use 0, otherwise use x.TagValue.Value
                    TagValue = x.TagValue ?? 0, // <-- *** FIX HERE ***
                    Timestamp = x.InDate.ToLocalTime()    // Convert InDate (presumably UTC) to Local Time
                })
                .OrderBy(m => m.Timestamp) // Order the final projected data by Timestamp
                .ToList(); // Execute the query and materialize the results into a list

            return Task.FromResult(data);
        }

        public Task<FlowRate> GetAverageFlowToNow(int lastHours,string tagName)
        {
            var since = DateTime.UtcNow.AddHours(-lastHours);
            var data = _db.Tags.Where(m => m.Name != null
            && m.Name == tagName
            && m.InDate >= since
            && m.InDate < DateTime.UtcNow).OrderBy(x=>x.InDate).ToList();
            decimal sum = 0;
            decimal _last = 0;
            if (data.Any())
            {
                var _first = data.First().TagValue;
                _last = data.Last().TagValue ?? 0;
                sum = (_last - _first) / 10 ?? 0;
            }
            

            var results = new FlowRate
            {
                Rate = sum,
                Name = tagName,
                Start = since.ToLocalTime().ToString("HH:mm"),
                End = DateTime.UtcNow.ToLocalTime().ToString("HH:mm"),
                Total = _last
            };

            return Task.FromResult(results);
        }

        public Task<FlowRate> GetAverageFlowToLastHours(int startHour, string tagName)
        {
            TimeZoneInfo localZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
            var lh = DateTime.Now;
            var hour_end = new DateTime(lh.Year, lh.Month, lh.Day, lh.Hour, 0, 0).ToUniversalTime();
            var _lh = DateTime.Now.AddHours(-startHour);
            var hour_start = new DateTime(_lh.Year, _lh.Month, _lh.Day, _lh.Hour, 0, 0).ToUniversalTime();
            var data = _db.Tags.Where(m => m.Name != null
                && m.Name == tagName
                && m.InDate >= hour_start
                && m.InDate < hour_end).OrderBy(m=>m.InDate).ToList();

            var results = new FlowRate
            {
                Name = "NA"
            };

            if (data.Any())
            {
                var _first = data.First().TagValue;
                var _last = data.Last().TagValue;
                var sum = _last - _first;
                var rate = (sum / startHour)/10;

                results = new FlowRate
                {
                    Rate = rate ?? 0,
                    Name = tagName,
                    Start = hour_start.ToLocalTime().ToString("HH:mm"),
                    End = hour_end.ToLocalTime().ToString("HH:mm"),
                    Total = _last ?? 0
                };
            }


            return Task.FromResult(results);

        }

        #region Get SelectListItems
        public Task<List<SelectListItem>> GetTagDefSelectList(string tagType, string category)
        {
            var results = new List<SelectListItem>();
            var data = new List<TagDefinition>();

            switch (category)
            {
                case "firstStageTemp":
                    data = _db.TagsDefinitions.Where(m => m.UnitType == tagType && (m.Category == "Input Feed" || m.Category == "1st Stage" || m.Category == "Feed" || m.Category == "Extruder" || m.Category=="Separator")).OrderBy(m=>m.UnitType).ToList();
                    break;
                case "pressure":
                    data = _db.TagsDefinitions.Where(m => m.UnitType == tagType && (m.Category == "Input Feed" || m.Category == "1st Stage" || m.Category=="Separator" || m.Category=="Extruder" || m.Category=="2nd Stage" || m.Category=="Feed")).OrderBy(m => m.UnitType).ToList();
                    break;
                case "secondStageTemp":
                    data = _db.TagsDefinitions.Where(m => m.UnitType == tagType && m.Category == "2nd Stage").OrderBy(m => m.UnitType).ToList();
                    break;
                case "cooling":
                    data = _db.TagsDefinitions.Where(m => m.UnitType == tagType && m.Category == "Cooling").OrderBy(m => m.UnitType).ToList();
                    break;
            }

            results = data.Select(x => new SelectListItem
            {
                Value = $"{x.UnitTag}",
                Text = $"{x.UnitTag} - {x.Description}"

            }).ToList();
            return Task.FromResult(results);

        }
        #endregion


    }
}
