using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.ComponentModel.DataAnnotations;

namespace Ignis.Data.DbModel
{
    public class AppUser
    {
        [Key]
        public int Id { get; set; }
        public string? UserId { get; set; }
        public int AccessLevel { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? MobileNumber { get; set; }
        public string? AccessCode { get; set; }
        public DateTime? InDate { get; set; }
        public DateTime? ModDate { get; set; }


    }
}
