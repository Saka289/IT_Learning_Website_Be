using System.Security.Cryptography;
using System.Text;

namespace LW.Infrastructure.Extensions;

public static class EncodeHelperExtensions
{
    public static string EncodeDocument(string bookCollection, string bookType, int publicationYear, int edition)
    {
        // CanhDieu, SGK, 2022, 1 -> CanhDieu-SGK-2022-01-xxxx
        var codeString = $"{bookCollection}-{bookType}-{publicationYear}-{edition:D2}";
        var result = codeString + "-" + GenerateHash(codeString);
        return result;
    }

    public static string Base64Encode(this string plainText)
    {
        var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
        return Convert.ToBase64String(plainTextBytes);
    }

    public static string Base64Decode(this string base64EncodedData)
    {
        var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
        return Encoding.UTF8.GetString(base64EncodedBytes);
    }

    public static bool IsBase64String(this string base64)
    {
        Span<byte> buffer = new Span<byte>(new byte[base64.Length]);
        return Convert.TryFromBase64String(base64, buffer, out int bytesParsed);
    }

    public static int HashArray(this string[] arr)
    {
        // Sort the array to ensure order does not affect the hash
        Array.Sort(arr);

        int hash = 0;
        for (int i = 0; i < arr.Length; i++)
        {
            string element = arr[i].Trim();
            for (int j = 0; j < element.Length; j++)
            {
                char charCode = element[j]; // Get character code for each character
                hash = (hash << 5) - hash + charCode; // Efficient bitwise XOR
                hash &= 0x7fffffff; // Ensure 32-bit hash value
            }
        }

        return hash;
    }


    private static string GenerateHash(string input)
    {
        using (SHA256 sha256Hash = SHA256.Create())
        {
            // Tạo mã băm từ chuỗi đầu vào
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < 4; i++) // Lấy 4 byte đầu tiên để tạo mã băm ngắn
            {
                builder.Append(bytes[i].ToString("X2"));
            }

            return builder.ToString();
        }
    }
}