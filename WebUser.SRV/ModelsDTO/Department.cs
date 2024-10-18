using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebUser.SRV.ModelsDTO
{
    public class Department
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DepartmentId { get; set; }

        [Required(ErrorMessage = "DepartmentName is required.")]
        public string DepartmentName { get; set; }

        // Colección de empleados en este departamento
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }

}

