using System;
using System.Threading.Tasks;

namespace WebUser.SRV.Interfaces
{
	public interface IEmployeeExportService
	{
        Task<string> ExportEmployeesToCsvAsync();
    }
}

