using Ignis.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ignis.Controllers
{
    public class ValidationController : Controller
    {
        private AppDbContext _db;

        public ValidationController(AppDbContext db) 
        { 
            _db = db;
        }

        public async Task<IActionResult> IsInviteEmailInUse(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                // Decide how to handle empty/whitespace input if needed, though [Required] should catch it first.
                // Returning true here means "valid" in the context of this specific check.
                return Json(true);
            }

            // Normalize email for consistent comparison (optional but recommended)
            var normalizedEmail = email.Trim().ToLowerInvariant();

            // Check if any user already has this email
            // Use async query for better performance
            var userExists = await _db.Invites.AnyAsync(m => email.ToLower() == normalizedEmail);


            // Remote validation expects 'true' if the field is VALID,
            // and 'false' or an error string if it is INVALID.
            // So, if the user *exists*, the email is *invalid* for registration.
            if (userExists)
            {
                // Option 1: Return false - jQuery Validate Unobtrusive will use the ErrorMessage from the [Remote] attribute.
                return Json(false);

                // Option 2: Return the error message directly (overrides the attribute's message)
                // return Json("This email address is already taken. Please choose another.");
            }
            else
            {
                // User does not exist, email is available and valid.
                return Json(true);
            }

        }

    }
}
