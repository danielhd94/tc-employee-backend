using System;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using WebUser.SRV.Interfaces;

namespace WebUser.SRV.Services
{
    public class EmployeeExportService : IEmployeeExportService
    {
        private readonly MyDbContext _dbContext;

        public EmployeeExportService(MyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<string> ExportEmployeesToCsvAsync()
        {
            var employees = await _dbContext.Employees
                .Include(e => e.Department)
                .Include(e => e.Gender)
                .ToListAsync();

            StringBuilder csvBuilder = new StringBuilder();
            csvBuilder.AppendLine("EmployeeCode,EmployeeName,DateOfJoining,Department,Gender");

            foreach (var employee in employees)
            {
                csvBuilder.AppendLine($"{employee.EmployeeCode},{employee.EmployeeName},{employee.DateOfJoining:yyyy-MM-dd},{employee.Department.DepartmentName},{employee.Gender.GenderName}");
            }

            string csvContent = csvBuilder.ToString();
            string filePath = Path.Combine("exports", "employees.csv");
            await File.WriteAllTextAsync(filePath, csvContent);

            return filePath;
        }
    }
}