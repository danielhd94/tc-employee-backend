using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using WebUser.SRV.ModelsDTO;
using WebUser.SRV.Response;

namespace WebUser.SRV.Interfaces
{
    public interface IEmployeeService
	{
        Task<TResponse<IEnumerable<EmployeeDTO>>> GetEmployeesAsync();
        Task<TResponse<EmployeeDTO>> GetEmployeeByIdAsync(int employeeId);
        Task<TResponse<EmployeeDTO>> CreateEmployeeAsync(EmployeeDTO emp);
        Task<TResponse<EmployeeDTO>> UpdateEmployeeAsync(EmployeeDTO emp);
        Task<TResponse<string>> DeleteEmployeeAsync(int id);
        Task<TResponse<string>> SaveFile(IFormFile file);
        Task<TResponse<IEnumerable<GenderCountDTO>>> GetGenderCountAsync();
    }
}

