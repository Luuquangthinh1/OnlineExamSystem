using System.Security.Cryptography;
using System.Text;

public static class HashHelper
{
    public static string Hash(string input)
    {
        using (SHA256 sha = SHA256.Create())
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}