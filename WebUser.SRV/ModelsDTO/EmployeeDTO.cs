using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebUser.SRV.ModelsDTO
{
    public class EmployeeDTO
    {
        public int EmployeeId { get; set; }

        public string EmployeeName { get; set; }

        public DateTime DateOfJoining { get; set; }

        public string PhotoFileName { get; set; }

        public DepartmentDTO Department { get; set; }

        public GenderDTO Gender { get; set; }

    }

}

