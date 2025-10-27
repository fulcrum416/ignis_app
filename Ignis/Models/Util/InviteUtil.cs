using Ignis.Data;
using Ignis.Data.DbModel;
using Ignis.Models.Commons;
using Microsoft.EntityFrameworkCore;

namespace Ignis.Models.Util
{
    public interface IInviteUtil
    {
        Task<List<Invite>> GetAllInvitesAsync();
        Task<Invite?> GetInviteDetailsAsync(string email);
    }
    public class InviteUtil : Ignis.Models.Commons.Util,IInviteUtil
    {
        private readonly AppDbContext _db;

        public InviteUtil(AppDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<Invite?> GetInviteDetailsAsync(string email)
        {
            var result = await _db.Invites.FirstOrDefaultAsync(x=>x.Email == email);
            return result;
        }

        public async Task<List<Invite>> GetAllInvitesAsync()
        {
            var results = await _db.Invites.ToListAsync();
            return results;
        }

    }
}
