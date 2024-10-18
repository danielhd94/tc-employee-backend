using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WebUser.SRV.Interfaces;
using WebUser.SRV.ModelsDTO;
using WebUser.SRV.Response;

namespace WebUser.SRV.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IConfiguration _configuration;
        private readonly MyDbContext _dbContext;

        public DepartmentService(IConfiguration configuration, MyDbContext dbContext)
        {
            _configuration = configuration;
            _dbContext = dbContext;
        }

        public async Task<TResponse<IEnumerable<DepartmentDTO>>> GetDepartmentAsync()
        {
            try
            {
                var departments = await _dbContext.Departments.ToListAsync();
                var departmentDTOs = departments.Select(d => new DepartmentDTO
                {
                    DepartmentId = d.DepartmentId,
                    DepartmentName = d.DepartmentName
                });

                return TResponse<IEnumerable<DepartmentDTO>>.Create(true, departmentDTOs, "Departments fetched successfully");
            }
            catch (Exception ex)
            {
                string detailedMessage = $"Error: {ex.Message}\nStackTrace: {ex.StackTrace}";
                return TResponse<IEnumerable<DepartmentDTO>>.Create(false, null, detailedMessage);
            }
        }

        public async Task<TResponse<DepartmentDTO>> GetDepartmentByIdAsync(int departmentId)
        {
            try
            {
                var department = await _dbContext.Departments
                    .FirstOrDefaultAsync(d => d.DepartmentId == departmentId);

                if (department != null)
                {
                    var departmentDTO = new DepartmentDTO
                    {
                        DepartmentId = department.DepartmentId,
                        DepartmentName = department.DepartmentName
                    };

                    return TResponse<DepartmentDTO>.Create(true, departmentDTO, "Department retrieved successfully.");
                }
                else
                {
                    return TResponse<DepartmentDTO>.Create(false, null, "Department not found.");
                }
            }
            catch (Exception ex)
            {
                string detailedMessage = $"Error: {ex.Message}\nStackTrace: {ex.StackTrace}";
                return TResponse<DepartmentDTO>.Create(false, null, detailedMessage);
            }
        }

        public async Task<TResponse<DepartmentDTO>> CreateDepartmentAsync(DepartmentDTO dep)
        {
            try
            {
                if (dep == null)
                {
                    return TResponse<DepartmentDTO>.Create(false, null, "Invalid department data.");
                }

                var departmentEntity = new Department
                {
                    DepartmentName = dep.DepartmentName
                };

                _dbContext.Departments.Add(departmentEntity);
                await _dbContext.SaveChangesAsync();

                var newDepartment = await _dbContext.Departments
                    .FirstOrDefaultAsync(d => d.DepartmentId == departmentEntity.DepartmentId);

                if (newDepartment != null)
                {
                    var newDepartmentDTO = new DepartmentDTO
                    {
                        DepartmentId = newDepartment.DepartmentId,
                        DepartmentName = newDepartment.DepartmentName
                    };

                    return TResponse<DepartmentDTO>.Create(true, newDepartmentDTO, "Department added successfully");
                }
                else
                {
                    return TResponse<DepartmentDTO>.Create(false, null, "Failed to retrieve the newly created department.");
                }
            }
            catch (Exception ex)
            {
                string detailedMessage = $"Error: {ex.Message}\nStackTrace: {ex.StackTrace}";
                return TResponse<DepartmentDTO>.Create(false, null, detailedMessage);
            }
        }

        public async Task<TResponse<DepartmentDTO>> UpdateDepartmentAsync(DepartmentDTO dep)
        {
            try
            {
                if (dep == null)
                {
                    return TResponse<DepartmentDTO>.Create(false, null, "Invalid department data.");
                }

                var department = await _dbContext.Departments
                    .FirstOrDefaultAsync(d => d.DepartmentId == dep.DepartmentId);

                if (department == null)
                {
                    return TResponse<DepartmentDTO>.Create(false, null, "Department not found.");
                }

                department.DepartmentName = dep.DepartmentName;
                await _dbContext.SaveChangesAsync();

                return TResponse<DepartmentDTO>.Create(true, dep, "Department updated successfully");
            }
            catch (Exception ex)
            {
                string detailedMessage = $"Error: {ex.Message}\nStackTrace: {ex.StackTrace}";
                return TResponse<DepartmentDTO>.Create(false, null, detailedMessage);
            }
        }

        public async Task<TResponse<string>> DeleteDepartmentAsync(int departmentId)
        {
            try
            {
                var department = await _dbContext.Departments
                    .FirstOrDefaultAsync(d => d.DepartmentId == departmentId);

                if (department == null)
                {
                    return TResponse<string>.Create(false, null, "Department not found.");
                }

                _dbContext.Departments.Remove(department);
                await _dbContext.SaveChangesAsync();

                return TResponse<string>.Create(true, "Deleted Successfully", "Department deleted successfully.");
            }
            catch (Exception ex)
            {
                string detailedMessage = $"Error: {ex.Message}\nStackTrace: {ex.StackTrace}";
                return TResponse<string>.Create(false, null, detailedMessage);
            }
        }
    }
}
