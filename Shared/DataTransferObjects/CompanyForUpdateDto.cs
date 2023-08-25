using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DataTransferObjects
{
    public record CompanyForUpdateDto
    {
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? Country { get; set; }
        public IEnumerable<EmployeeForCreationDto>? Employees { get; set; }
    }
}
