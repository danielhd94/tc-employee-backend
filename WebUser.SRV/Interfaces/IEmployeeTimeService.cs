using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure;
using WebUser.SRV.ModelsDTO;
using WebUser.SRV.Response;

namespace WebUser.SRV.Interfaces
{
	public interface IEmployeeTimeService
	{
        Task<TResponse<IEnumerable<EmployeeTimeDTO>>> GetEmployeeTimesAsync();
        Task<TResponse<EmployeeTimeDTO>> GetEmployeeTimeByIdAsync(int employeeTimeId);
        Task<TResponse<EmployeeTimeDTO>> CreateEmployeeTimeAsync(EmployeeTimeDTO employeeTimeDTO);
        Task<TResponse<EmployeeTimeDTO>> UpdateEmployeeTimeAsync(EmployeeTimeDTO employeeTimeDTO);
        Task<TResponse<string>> DeleteEmployeeTimeAsync(int employeeTimeId);
        Task<TResponse<IEnumerable<EmployeeTimeDTO>>> AddTimeRecordsAsync(IEnumerable<EmployeeTimeDTO> timeRecords);
    }

}

