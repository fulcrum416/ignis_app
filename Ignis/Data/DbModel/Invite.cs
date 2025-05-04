using System.ComponentModel.DataAnnotations;

namespace Ignis.Data.DbModel
{
    public class Invite
    {
        [Key]
        public int Id { get; set; } 
        public string? Email { get; set; }
        public string? MobileNumber { get; set; }
        public bool IsActive { get; set; }
        public int TotalDays { get; set; }
        public bool NoLimit { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? TempCode { get; set; }
        public string? ConfirmationCode { get; set; }
        public DateTime? InviteDate { get; set; }
        public DateTime? InDate { get; set; }
        public DateTime? ModDate { get; set; }

        
    }
}
