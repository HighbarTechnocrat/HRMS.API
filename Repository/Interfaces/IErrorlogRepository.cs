using HRMS.API.Models;

namespace HRMS.API.Repository.Interfaces
{
    public interface IErrorlogRepository
    {
        Task  CreateLogErrorAsync(ErrorLog errorLog);
   
    }
}
