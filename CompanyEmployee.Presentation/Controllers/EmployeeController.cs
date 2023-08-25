using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyEmployees.Presentation.Controllers
{
    [Route("api/companies/{companyId}/employees")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IServiceManager _service;

        public EmployeeController(IServiceManager service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult GetEmploees(Guid companyId)
        {
            var emploees = _service.EmploeeService.GetEmployees(companyId, trackChanges: false);

            return Ok(emploees);
        }

        [HttpGet("{id:guid}", Name = "GetEmployeeForCompany")]
        public IActionResult GetEmploees(Guid companyId, Guid id)
        {
            var emploee = _service.EmploeeService.GetEmployee(companyId, id, trackChanges: false);

            return Ok(emploee);
        }

        [HttpPost]
        public IActionResult CreateEmployeeForCompany(Guid companyId, [FromBody] EmployeeForCreationDto employee)
        {
            if (employee is null)
            {
                return BadRequest("EmployeeForCreationDto object is null");
            }
            
            var emploeeToReturn = _service.EmploeeService.CreateEmployeeForCompany(companyId, employee, trackChanges:false);

            return CreatedAtRoute("GetEmployeeForCompany", new { companyId, id = emploeeToReturn.Id }, emploeeToReturn);
        }

        [HttpDelete("{id:guid}")]
        public IActionResult DeleteEmployeeForCompany(Guid companyId, Guid id) 
        {
            _service.EmploeeService.DeleteEmployeeForCompany(companyId, id, trackChanges: false);

            return NoContent();
        }

        [HttpPut("{id:guid}")]
        public IActionResult UpdateEmployee(Guid companyId, Guid id, [FromBody] EmployeeForUpdateDto employee)
        {
            if (employee is null)
            {
                return BadRequest("EmploeeForUpdateDto object is null");
            }

            _service.EmploeeService.UpdateEmployeeForCompany(companyId, id, employee, companyTrackChanges: false, employeeTrackChanges: true);
            
            return NoContent();
        }
    }
}
