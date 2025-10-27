using Ignis.Data.DbModel;
using Ignis.Models.FormModels;

namespace Ignis.Models.Bindings
{
    public class SettingBindings
    {
        public List<Invite> Invites { get; set; }= new List<Invite>();        
        public InviteFormModel InviteForm { get; set; } = new InviteFormModel();
    }
}
