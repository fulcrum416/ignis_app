using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Ignis.Models.Commons
{
    public static class Extension
    {
        /// <summary>
        /// Creates SelectListItem where Text is the enum member's code name (e.g., "Lowest") and 
        /// Value is its underlying integer (e.g., "0"). Simple and direct mapping.
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="selectedValue"></param>
        /// <returns></returns>
        public static IEnumerable<SelectListItem> EnumToSelectListBasic<TEnum>(TEnum? selectedValue = null)
    where TEnum : struct, Enum // Constraint: TEnum must be a value type and an Enum
        {
            // Get all values from the enum
            var values = Enum.GetValues(typeof(TEnum)).Cast<TEnum>();

            // Project them into SelectListItem objects
            return values.Select(e => new SelectListItem
            {
                // Text is the enum member's name (e.g., "Lowest", "Low")
                Text = e.ToString(),

                // Value is the underlying integer, converted to string (e.g., "0", "1")
                Value = Convert.ToInt32(e).ToString(),

                // Set Selected flag if the current enum value matches the provided selectedValue
                Selected = selectedValue.HasValue && EqualityComparer<TEnum>.Default.Equals(e, selectedValue.Value)
            });
        }
        /// <summary>
        /// Creates SelectListItem where both Text and Value are the enum member's code name 
        /// (e.g., Text="Medium", Value="Medium"). Useful for binding directly back to enum properties by name.
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="selectedValue"></param>
        /// <returns></returns>
        public static IEnumerable<SelectListItem> EnumToSelectListWithNameAsValue<TEnum>(TEnum? selectedValue = null)
    where TEnum : struct, Enum
        {
            return Enum.GetValues(typeof(TEnum))
                       .Cast<TEnum>()
                       .Select(e => new SelectListItem
                       {
                           // Text is the enum member's name
                           Text = e.ToString(),

                           // Value is also the enum member's name
                           Value = e.ToString(),

                           Selected = selectedValue.HasValue && EqualityComparer<TEnum>.Default.Equals(e, selectedValue.Value)
                       });
        }

        /// <summary>
        /// Creates SelectListItem where Text comes from the [Display(Name = "...")] attribute for user-friendly display 
        /// (e.g., "Medium (Default)"), falling back to the code name if the attribute is missing. 
        /// The Value can be configured to be either the underlying integer or the enum member's code name. 
        /// Most flexible for UI presentation.
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetDisplayName<TEnum>(this TEnum value)
            where TEnum : struct, Enum
        {
            // Get the MemberInfo object for the enum value
            MemberInfo? memberInfo = typeof(TEnum)
                .GetMember(value.ToString())
                .FirstOrDefault();

            if (memberInfo != null)
            {
                // Check for Display attribute
                DisplayAttribute? displayAttribute = memberInfo
                    .GetCustomAttribute<DisplayAttribute>();

                if (displayAttribute != null)
                {
                    // Return the Name property of the attribute, or fallback if null
                    // GetName() handles localization if ResourceType is set.
                    return displayAttribute.GetName() ?? value.ToString();
                }
            }

            // Fallback if no MemberInfo or Display attribute found
            return value.ToString();
        }

        public static IEnumerable<SelectListItem> EnumToSelectListWithDisplay<TEnum>(TEnum? selectedValue = null, bool useIntValue = true)
            where TEnum : struct, Enum
        {
            return Enum.GetValues(typeof(TEnum))
                       .Cast<TEnum>()
                       .Select(e => new SelectListItem
                       {
                           // Use the helper method to get the display name
                           Text = e.GetDisplayName(),

                           // Choose value based on parameter: integer or name string
                           Value = useIntValue ? Convert.ToInt32(e).ToString() : e.ToString(),

                           Selected = selectedValue.HasValue && EqualityComparer<TEnum>.Default.Equals(e, selectedValue.Value)
                       });
        }
    }
}

