using System.Security.Cryptography;
using System.Text;

namespace HRMS.API.Helper
{
    public class PasswordHelper
    {
        private static readonly Random random = new();

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789@";
            return new string(
                Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray()
            );
        }

        public static string HashSHA1(string input)
        {
            using var sha1 = SHA1.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = sha1.ComputeHash(bytes);
            return BitConverter.ToString(hash).Replace("-", "");
        }
    }
}
