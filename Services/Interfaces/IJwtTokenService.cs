using HRMS.API.Models;

namespace HRMS.API.Services.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateToken(User user);
    }
}
