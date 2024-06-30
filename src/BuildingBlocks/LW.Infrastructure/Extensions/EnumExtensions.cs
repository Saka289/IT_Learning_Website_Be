using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace LW.Infrastructure.Extensions;

public static class EnumExtensions
{
    public static string? GetDisplayName(Enum enumValue)
    {
        return enumValue.GetType()
            .GetMember(enumValue.ToString())
            .First()
            .GetCustomAttribute<DisplayAttribute>()
            ?.GetName();
    }
}