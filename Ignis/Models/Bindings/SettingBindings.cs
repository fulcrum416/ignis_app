using Ignis.Data.DbModel;

namespace Ignis.Models.Bindings
{
    public class SettingBindings
    {
        public List<Invite> Invites { get; set; }= new List<Invite>();
        public Invite InviteData { get; set; } = new Invite();
    }
}
