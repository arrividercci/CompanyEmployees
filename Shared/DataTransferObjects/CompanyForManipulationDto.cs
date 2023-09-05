using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DataTransferObjects
{
    public abstract record CompanyForManipulationDto
    {
        [Required(ErrorMessage = "Company Name is required field.")]
        [MaxLength(30, ErrorMessage = "Maximum length for the Name is 30 characters.")]
        public string? Name { get; init; }
        [Required(ErrorMessage = "Company Address is required field.")]
        [MaxLength(50, ErrorMessage = "Maximum length for the Adress is 50 characters.")]
        public string? Address { get; init; }
        [Required(ErrorMessage = "Company Country is required field.")]
        [MaxLength(20, ErrorMessage = "Maximum length for the Country is 50 characters.")]
        public string? Country { get; init; }
        public IEnumerable<EmployeeForCreationDto>? Employees { get; set; }
    }
}
