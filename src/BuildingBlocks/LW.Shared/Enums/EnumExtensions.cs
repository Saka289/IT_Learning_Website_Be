using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LW.Shared.Enums
{
    public static class EnumExtensions
    {
        public static string GetDisplayNameEnum(this Enum enumValue)
        {
            var displayAttribute = enumValue.GetType()
                .GetMember(enumValue.ToString())
                .First()
                .GetCustomAttribute<DisplayAttribute>();

            return displayAttribute?.Name ?? enumValue.ToString();
        }
        // Helper method to get integer value from enum display name
        public static int GetEnumIntValueFromDisplayName<TEnum>(string displayName) where TEnum : Enum
        {
            // Get all enum values
            foreach (TEnum enumValue in Enum.GetValues(typeof(TEnum)))
            {
                // Retrieve the display attribute
                DisplayAttribute displayAttribute = enumValue.GetType()
                                                            .GetMember(enumValue.ToString())
                                                            .FirstOrDefault()
                                                            ?.GetCustomAttribute<DisplayAttribute>();

                // Check if display name matches
                if (displayAttribute != null && displayAttribute.Name == displayName)
                {
                    // Return the integer value of the enum
                    return Convert.ToInt32(enumValue);
                }
            }

            return 0; // Return 0 if not found
        }
    }
}
