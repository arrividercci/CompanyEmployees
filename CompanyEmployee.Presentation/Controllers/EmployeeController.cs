using Microsoft.AspNetCore.JsonPatch;
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
        public async Task<IActionResult> GetEmploees(Guid companyId)
        {
            var emploees = await _service.EmployeeService.GetEmployeesAsync(companyId, trackChanges: false);

            return Ok(emploees);
        }

        [HttpGet("{id:guid}", Name = "GetEmployeeForCompany")]
        public async Task<IActionResult> GetEmploees(Guid companyId, Guid id)
        {
            var emploee = await _service.EmployeeService.GetEmployeeAsync(companyId, id, trackChanges: false);

            return Ok(emploee);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployeeForCompany(Guid companyId, [FromBody] EmployeeForCreationDto employee)
        {
            if (employee is null)
            {
                return BadRequest("EmployeeForCreationDto object is null");
            }
            
            var emploeeToReturn = await _service.EmployeeService.CreateEmployeeForCompanyAsync(companyId, employee, trackChanges:false);

            return CreatedAtRoute("GetEmployeeForCompany", new { companyId, id = emploeeToReturn.Id }, emploeeToReturn);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteEmployeeForCompany(Guid companyId, Guid id) 
        {
            await _service.EmployeeService.DeleteEmployeeForCompanyAsync(companyId, id, trackChanges: false);

            return NoContent();
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateEmployee(Guid companyId, Guid id, [FromBody] EmployeeForUpdateDto employee)
        {
            if (employee is null)
            {
                return BadRequest("EmploeeForUpdateDto object is null");
            }

            await _service.EmployeeService.UpdateEmployeeForCompanyAsync(companyId, id, employee, companyTrackChanges: false, employeeTrackChanges: true);
            
            return NoContent();
        }

        [HttpPatch("{id:guid}")]
        public async Task<IActionResult> PartiallyUpdateEmployeeForCompany(Guid companyId, Guid id, [FromBody] JsonPatchDocument<EmployeeForUpdateDto> patchDoc)
        {
            if (patchDoc is null)
                return BadRequest("patchDoc object sent from client is null.");
            
            var result = await _service.EmployeeService.GetEmployeeForPatchAsync(companyId, id, compTrackChanges: false, empTrackChanges: true);
            
            patchDoc.ApplyTo(result.employeeToPatch);
            
            await _service.EmployeeService.SaveChangesForPatchAsync(result.employeeToPatch, result.employeeEntity);
            
            return NoContent();
        }

    }
}
