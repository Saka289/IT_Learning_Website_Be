using System.Security.Cryptography;
using System.Text;

namespace LW.Infrastructure.Extensions;

public static class EncodeHelperExtensions
{
    public static string EncodeDocument(string bookCollection,string bookType, int publicationYear, int edition)
    {
        // CanhDieu, SGK, 2022, 1 -> CanhDieu-SGK-2022-01-xxxx
        var codeString = $"{bookCollection}-{bookType}-{publicationYear}-{edition:D2}";
        var result = codeString +"-"+ GenerateHash(codeString);
        return result;
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