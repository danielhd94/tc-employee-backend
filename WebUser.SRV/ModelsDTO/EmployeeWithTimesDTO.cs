using System;
using System.Collections.Generic;

namespace WebUser.SRV.ModelsDTO
{
	public class EmployeeWithTimesDTO
	{
        public EmployeeDTO Employee { get; set; }
        public IEnumerable<EmployeeTimeDTO> Times { get; set; }
    }
}

