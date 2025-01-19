using Ignis.Models;
using Ignis.Models.Bindings;
using Ignis.Models.Props;
using Ignis.Models.Util;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

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
            var model = new HomeBindings
            {
                Current = currentFlow,
                Last3Hours = lastThreeHrsRate,
                LastHour = lastHoursRate
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

        public async Task<IActionResult> GetTagData(string tags="X")
        {
            if (!string.IsNullOrEmpty(tags))
            {
                // Set Central Time
                var centralTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");


                var defaultTags = new List<string> {"TT-303","R1-PH" };
                var tagDatas = await _defs.GetTagData(3,defaultTags);
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
