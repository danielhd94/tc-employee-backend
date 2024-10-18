using System;
using System.Reflection;
using AutoMapper;
using WebUser.SRV.ModelsDTO;

namespace WebUser.Mapper
{
    public class AutoMapperConfiguration : Profile
	{
		public AutoMapperConfiguration()
		{
            //CreateMap<Employee, EmployeeDTO>().ReverseMap();
            //CreateMap<EmployeeDTO, Employee>().ReverseMap();

            // Perfil para mapear Employee a EmployeeDTO
            CreateMap<Employee, EmployeeDTO>()
                .ForMember(dest => dest.Department, opt => opt.MapFrom(src => src.Department))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender));

            CreateMap<Department, DepartmentDTO>().ReverseMap();
            CreateMap<Gender, GenderDTO>().ReverseMap();
        }
	}
}

