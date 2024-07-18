namespace LW.Infrastructure.Extensions;

public static class TagValueHelperExtensions
{
    public static string ConvertToTagString(this IEnumerable<string> tagValues)
    {
        var result = "";
        if (tagValues != null && tagValues.Any())
        {
            result = string.Join(",", tagValues);
        }
        return result;
    }
}