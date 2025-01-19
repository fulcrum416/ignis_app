using Ignis.Data;
using Ignis.Data.DbModel;
using Ignis.Models.Props;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace Ignis.Models.Util
{
    public interface ITagDefs
    {
        Task<FlowRate> GetAverageFlowToLastHours(int startHour, string tagName);
        Task<FlowRate> GetAverageFlowToNow(int lastHours, string tagName);
        Task<List<SelectListItem>> GetTagCatgoriesSelectListAsync(string selectedText="");
        Task<List<TagChartData>> GetTagData(int totalHours, List<string> tagNames);
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
            var data = _db.Tags.Where(m => m.Name != null
            && tagNames.Contains(m.Name)
            && m.LogDate >= since
            && m.LogDate <= DateTime.UtcNow
            ).OrderBy(x=>x.InDate).Select(x=>new TagChartData
            {
                Name = x.Name,
                TagValue=x.TagValue.Value,
                Timestamp=x.InDate.ToLocalTime()
            }).OrderBy(m=>m.Timestamp).ToList();

            return Task.FromResult(data);
        }

        public Task<FlowRate> GetAverageFlowToNow(int lastHours,string tagName)
        {
            var since = DateTime.UtcNow.AddHours(-lastHours);
            var data = _db.Tags.Where(m => m.Name != null
            && m.Name == tagName
            && m.InDate >= since
            && m.InDate < DateTime.UtcNow).OrderBy(x=>x.InDate).ToList();

            var _first = data.First().TagValue;
            var _last = data.Last().TagValue;
            var sum = _last - _first;

            var results = new FlowRate
            {
                Rate = sum ?? 0,
                Name = tagName,
                Start = since.ToLocalTime().ToString("HH:mm"),
                End = DateTime.UtcNow.ToLocalTime().ToString("HH:mm"),
                Total = _last??0
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
                var rate = sum / startHour;

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


    }
}
