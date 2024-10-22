using System;
using System.Reflection;
using AutoMapper;
using WebUser.SRV.ModelsDTO;
using WebUser.SRV.Models;

namespace WebUser.Mapper
{
    public class AutoMapperConfiguration : Profile
	{
		public AutoMapperConfiguration()
		{

            // Perfil para mapear Employee a EmployeeDTO
            CreateMap<Employee, EmployeeDTO>()
                .ForMember(dest => dest.Department, opt => opt.MapFrom(src => src.Department))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender));

            CreateMap<Department, DepartmentDTO>().ReverseMap();
            CreateMap<Gender, GenderDTO>().ReverseMap();
            CreateMap<EmployeeTime, EmployeeTimeDTO>()
            .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.Employee.EmployeeId))
            .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee.EmployeeName))
            .ForMember(dest => dest.EmployeeCode, opt => opt.MapFrom(src => src.Employee.EmployeeCode))
            .ReverseMap();
        }
	}
}

