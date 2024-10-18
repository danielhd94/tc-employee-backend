using System.Collections.Generic;
using System.Threading.Tasks;
using WebUser.SRV.ModelsDTO;
using WebUser.SRV.Response;

namespace WebUser.SRV.Interfaces
{
    public interface IDepartmentService
	{
        Task<TResponse<IEnumerable<DepartmentDTO>>> GetDepartmentAsync();
        Task<TResponse<DepartmentDTO>> GetDepartmentByIdAsync(int departmentId);
        Task<TResponse<DepartmentDTO>> CreateDepartmentAsync(DepartmentDTO dep);
        Task<TResponse<DepartmentDTO>> UpdateDepartmentAsync(DepartmentDTO dep);
        Task<TResponse<string>> DeleteDepartmentAsync(int id);
    }
}

