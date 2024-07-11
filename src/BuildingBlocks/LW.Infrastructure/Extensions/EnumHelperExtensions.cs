using System.ComponentModel.DataAnnotations;
using System.Reflection;
using LW.Shared.DTOs.Enum;
using LW.Shared.Enums;

namespace LW.Infrastructure.Extensions;

public static class EnumHelperExtensions
{
    public static string? GetDisplayName(Enum enumValue)
    {
        return enumValue.GetType()
            .GetMember(enumValue.ToString())
            .First()
            .GetCustomAttribute<DisplayAttribute>()
            ?.GetName();
    }
    public static string GetDisplayNameEnum(this Enum enumValue)
    {
        var displayAttribute = enumValue.GetType()
            .GetMember(enumValue.ToString())
            .First()
            .GetCustomAttribute<DisplayAttribute>();

        return displayAttribute?.Name ?? enumValue.ToString();
    }
    //// Helper method to get integer value from enum display name


    public static IEnumerable<EnumDto> ToEnumDto(this Type enumType)
    {
        var enumValues = Enum.GetValues(enumType).Cast<Enum>();
        var result = enumValues.Select(e => new EnumDto
        {
            Value = Convert.ToInt32(e),
            Name = GetDisplayName(e) ?? e.ToString()
        }).ToList();

        return result;
    }
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