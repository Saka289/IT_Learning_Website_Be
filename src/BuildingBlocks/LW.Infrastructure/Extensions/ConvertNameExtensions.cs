namespace LW.Infrastructure.Extensions;

public static class ConvertNameExtensions
{
    public static string ConvertRoleName(this string roleName)
    {
        if (RoleTranslations.TryGetValue(roleName, out var convertRoleName))
        {
            return convertRoleName;
        }

        return roleName;
    }

    private static readonly Dictionary<string, string> RoleTranslations = new()
    {
        { "Admin", "Quản trị viên" },
        { "User", "Người dùng" },
        { "ContentManager", "Quản trị nội dung" },
        { "Teacher", "Giáo viên" },
        { "Parent", "Phụ huynh" }
    };
}