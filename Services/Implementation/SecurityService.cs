using System.Text;
using System.Security.Cryptography;  

namespace HRMS.API.Services.Implementation
{
    public class SecurityService : ISecurityService
    {
        private readonly ILogger<SecurityService> _logger;

        public SecurityService(ILogger<SecurityService> logger)
        {
            _logger = logger;
        }

        public string HashSHA1(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            try
            {
                using var sha1 = SHA1.Create();
                var inputBytes = Encoding.UTF8.GetBytes(value);
                var hash = sha1.ComputeHash(inputBytes);

                var sb = new StringBuilder();
                foreach (var b in hash)
                {
                    sb.Append(b.ToString("X2"));
                }

                return sb.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error computing SHA1 hash for value: {Value}", value);
                throw new ApplicationException("Error computing hash", ex);
            }
        }

        public string HashSHA256(string value)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(value);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        public bool VerifySHA1(string input, string hashedValue)
        {
            var hashedInput = HashSHA1(input);
            return hashedInput.Equals(hashedValue, StringComparison.OrdinalIgnoreCase);
        }
    }
}
