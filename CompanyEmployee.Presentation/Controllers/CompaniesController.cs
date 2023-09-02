using CompanyEmployees.Presentation.ModelBinders;
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
    [Route("api/[controller]")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly IServiceManager service;

        public CompaniesController(IServiceManager service)
        {
            this.service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetCompaniesAsync()
        {
            var companies = await service.CompanyService.GetAllCompaniesAsync(trackChanges: false);

            return Ok(companies);
        }
        [HttpGet("{id:guid}", Name = "CompanyById")]
        public async Task<IActionResult> GetCompany(Guid id) 
        {
            var company = await service.CompanyService.GetCompanyAsync(id, trackChanges: false);

            return Ok(company);
        }

        [HttpPost(Name = "CreateCompany")]
        public async Task<IActionResult> CreateCompanyAsync([FromBody] CompanyForCreationDto company)
        {
            if (company is null)
            {
                return BadRequest("CompanyForCreationDto object is null");
            }

            var createdCompany = await service.CompanyService.CreateCompanyAsync(company);

            return CreatedAtRoute("CompanyById", new {id = createdCompany.Id}, createdCompany);
        }

        [HttpGet("collection/({ids})", Name = "CompanyCollection")]
        public async Task<IActionResult> GetCompanyCollectionAsync([ModelBinder(BinderType = typeof(ArrayModelBinder))]IEnumerable<Guid> ids)
        {
            var companies = await service.CompanyService.GetByIdsAsync(ids, trackChanges: false);

            return Ok(companies);
        }

        [HttpPost("collection")]
        public async Task<IActionResult> CreateCompanyCollectionAsync([FromBody] IEnumerable<CompanyForCreationDto> companies)
        {
            var result = await service.CompanyService.CreateCompanyCollectionAsync(companies);

            return CreatedAtRoute("CompanyCollection", new { result.ids }, result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteCompanyAsync(Guid id)
        {
            await service.CompanyService.DeleteCompanyAsync(id, trackChanges: false);

            return NoContent();
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateCompanyAsync(Guid id, [FromBody] CompanyForUpdateDto company)
        {
            if (company is null)
            {
                return BadRequest("CompanyForUpdateDto object is null");
            }

            await service.CompanyService.UpdateCompanyAsync(id, company, trackChanges: true);

            return NoContent();
        }
    }
}
