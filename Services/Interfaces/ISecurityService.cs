 

namespace HRMS.API.Services
{
    public interface ISecurityService
    {
        string HashSHA1(string value);
        string HashSHA256(string value);
        bool VerifySHA1(string input, string hashedValue);
    }

    
}