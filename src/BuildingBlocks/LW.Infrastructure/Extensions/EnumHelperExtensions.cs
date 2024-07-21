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
}