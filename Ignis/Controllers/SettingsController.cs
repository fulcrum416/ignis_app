using AspNetCoreGeneratedDocument;
using Ignis.Models.Bindings;
using Ignis.Models.Util;
using Microsoft.AspNetCore.Mvc;

namespace Ignis.Controllers
{
    public class SettingsController : Controller
    {
        private ILogger<SettingsController> _logger;
        private IInviteUtil _invite;

        public SettingsController(ILogger<SettingsController> logger, IInviteUtil invite) 
        {
            _logger = logger;
            _invite = invite;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Invitation()
        {
            var model = new SettingBindings
            {
                Invites = await _invite.GetAllInvitesAsync()
            };
            return View(model);
        }

        #region AJAX REQUEST
        public Task<IActionResult> NewInvite(int id = 0)
        {
            if (id == 0)
            {
                // new invite logic

            }
            else
            {
                // existing invite logic
            }
        }
        #endregion
    }
}
