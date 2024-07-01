using System.ComponentModel.DataAnnotations;
using System.Reflection;
using LW.Shared.Enums;

namespace LW.Infrastructure.Extensions;

public static class EnumHelperExtensions
{
    public static string GetDisplayName(Enum enumValue)
    {
        return enumValue.GetType()
            .GetMember(enumValue.ToString())
            .First()
            .GetCustomAttribute<DisplayAttribute>()
            ?.GetName();
    }
}