using Ignis.Models;
using Ignis.Models.Bindings;
using Ignis.Models.Props;
using Ignis.Models.Util;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace Ignis.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ITagDefs _defs;
        private DateTime _localTime;

        public HomeController(ILogger<HomeController> logger, ITagDefs defs)
        {
            _logger = logger;
            _defs = defs;

            TimeZoneInfo localZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
            DateTime utcNow = DateTime.UtcNow;
            _localTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, localZone);
        }

        public async Task<IActionResult> Index()
        {
            var tag = "FI-100-Total";
            var currentFlow = await _defs.GetAverageFlowToNow(1, tag);
            var lastHoursRate = await _defs.GetAverageFlowToLastHours(2, tag);
            var lastThreeHrsRate = await _defs.GetAverageFlowToLastHours(3, tag);

            var firstStage = await _defs.GetTagDefSelectList("Temp", "firstStageTemp");
            var pressure = await _defs.GetTagDefSelectList("Pressure", "pressure");
            var secondStage = await _defs.GetTagDefSelectList("Temp", "secondStageTemp");
            var cooling = await _defs.GetTagDefSelectList("Temp", "cooling");
            var model = new HomeBindings
            {
                Current = currentFlow,
                Last3Hours = lastThreeHrsRate,
                LastHour = lastHoursRate,
                FirstStageTempTags = firstStage,
                SecondStageTempTags = secondStage,
                PressureTags = pressure,
                CoolingTags = cooling
                
            };
            return View(model);
        }

        public async Task<IActionResult> GetFeedFlowRate()
        {
            var tag = "FI-100-Total";
            var currentFlow = await _defs.GetAverageFlowToNow(1, tag);
            var lastHoursRate = await _defs.GetAverageFlowToLastHours(2, tag);
            var lastThreeHrsRate = await _defs.GetAverageFlowToLastHours(3, tag);
            var model = new HomeBindings
            {
                Current = currentFlow,
                Last3Hours = lastThreeHrsRate,
                LastHour = lastHoursRate
            };

            return PartialView("_FlowRate", model);
        }

        public async Task<IActionResult> GetFlowRate()
        {
            try
            {
                var tag = "FI-100-Total";
                var currentFlow = await _defs.GetAverageFlowToNow(1, tag);
                var lastHoursRate = await _defs.GetAverageFlowToLastHours(2, tag);
                var lastThreeHrsRate = await _defs.GetAverageFlowToLastHours(3, tag);

                var data = new
                {
                    CurrentRate = currentFlow.Rate.ToString("0.00") ?? "0.0",
                    CurrentTotal = currentFlow.Total.ToString("0.00") ?? "0.0",
                    Last2Hours = lastHoursRate.Rate.ToString("0.00") ?? "0.0",
                    Last2HoursTotal = lastHoursRate.Total.ToString("0.00") ?? "0.0",
                    Last3Hours = lastThreeHrsRate.Rate.ToString("0.00") ?? "0.0",
                    Last3HoursTotal = lastThreeHrsRate.Total.ToString("0.00") ?? "0.0"
                };
                return Json(data);

            }catch (Exception ex)
            {

                var data = new
                {
                    CurrentRate = "0.0",
                    CurrentTotal ="0.0",
                    Last2Hours = "0.0",
                    Last2HoursTotal = "0.0",
                    Last3Hours = "0.0",
                    Last3HoursTotal = "0.0"
                };
                return Json(data);
            }




        }

        public async Task<IActionResult> GetTagData(string types, List<string> tags, int hours=3)
        {
            if (!string.IsNullOrEmpty(types))
            {
                // Set Central Time
                var centralTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
                
                var defaultTags = new List<string>();

                switch (types)
                {
                    case "1stStageTemp":
                        defaultTags = new List<string> { "TT-600", "TT-601", "ST-118", "TT-103", "ST-120" };
                        break;
                    case "1stStagePressure":
                        defaultTags = new List<string> { "PIT-100", "PIT-101", "PIT-104", "PIT-X200", "PIT-X201" };
                        break;
                    case "2ndStageTemp":
                        defaultTags = new List<string> { "ST-107", "ST-117", "ST-110", "ST-111", "D1-TEMP","D1-OVER" };
                        break;
                    case "coolingTemp":
                        defaultTags = new List<string> { "TT-128", "ST-115", "ST-114", "TT-129", "ST-201" };
                        break;
                }

                if(tags !=null && tags.Any())
                {
                    defaultTags = tags;
                }
                var tagDatas = await _defs.GetTagData(hours,defaultTags);
                //var groupData = tagDatas.GroupBy(x => x.Name??"").ToDictionary(g => g.Key,g => g.Select(d => new { d.Timestamp, d.TagValue }));
                // Prepare data: group by tag and convert timestamps to 15-second intervals
                var groupData = tagDatas
                 .GroupBy(d => d.Name ?? "")
                 .ToDictionary(
                     g => g.Key,
                     g => g.Select(d => new object[]
                     {
                        TimeZoneInfo.ConvertTimeFromUtc(d.Timestamp, centralTimeZone).Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds, // Central Time
                        d.TagValue
                     }).ToList()
                 );

                return Json(groupData);
            }

            return Ok();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
