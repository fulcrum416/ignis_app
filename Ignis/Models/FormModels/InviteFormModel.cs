using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Ignis.Models.FormModels
{
    public class InviteFormModel
    {
        public int InviteID { get;set; }
        public bool EditMode { get;set; }

        [Required]
        [EmailAddress(ErrorMessage ="Please enter a valid email address")]
        [Display(Name ="Email")]        
        [Remote(action:"IsInviteEmailInUse",controller:"Validation",HttpMethod ="POST", ErrorMessage ="Invite Already Exist")]
        public string? Email { get;set; }

        [Required]
        [Display(Name = "First Name")]
        public string? FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string? LastName { get; set; }

        [Display(Name ="User Access Level")]
        public List<SelectListItem>? AccessLevelList { get; set; }

        [Display(Name = "Total Days of Access")]
        public int TotalDays { get; set; } = 7;

        [Display(Name = "Unlimited Access (Infinite Amount of Access)?")]
        public bool IsUnlimited { get; set; } = false;

        [Required]
        public int SelectedAccessLevel { get; set; }
    }
}
