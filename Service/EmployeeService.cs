using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    internal sealed class EmployeeService : IEmployeeService
    {
        private readonly IRepositoryManager repository;
        private readonly ILoggerManager logger;
        private readonly IMapper _mapper;

        public EmployeeService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
        {
            this.repository = repository;
            this.logger = logger;
            this._mapper = mapper;
        }

        public async Task<EmployeeDto> CreateEmployeeForCompanyAsync(Guid companyId, EmployeeForCreationDto employee, bool trackChanges)
        {
            var company = await repository.Company.GetCompanyAsync(companyId, trackChanges);

            if (company is null)
            {
                throw new CompanyNotFoundException(companyId);
            }

            var employeeEntity = _mapper.Map<Employee>(company);

            repository.Employee.CreateEmployeeForCompany(companyId, employeeEntity);

            await repository.SaveAsync();

            var employeeToReturn = _mapper.Map<EmployeeDto>(employee);

            return employeeToReturn;
        }

        public async Task DeleteEmployeeForCompanyAsync(Guid companyId, Guid id, bool trackChanges)
        {
            var company = await repository.Company.GetCompanyAsync(companyId, trackChanges);

            if (company is null)
            {
                throw new CompanyNotFoundException(companyId);
            }

            var employeeForCompany = await repository.Employee.GetEmployeeAsync(companyId, id, trackChanges);

            if (employeeForCompany is null)
            {
                throw new EmployeeNotFoundException(id);
            }

            repository.Employee.DeleteEmployee(employeeForCompany);
            
            await repository.SaveAsync();
        }

        public async Task<EmployeeDto> GetEmployeeAsync(Guid companyId, Guid id, bool trackChanges)
        {
            var company = await repository.Company.GetCompanyAsync(companyId, trackChanges);

            if (company == null)
            {
                throw new CompanyNotFoundException(companyId);
            }

            var emploee = await repository.Employee.GetEmployeeAsync(companyId, id, trackChanges);

            if (emploee == null)
            {
                throw new EmployeeNotFoundException(id);
            }

            var employeeDto = _mapper.Map<EmployeeDto>(emploee);

            return employeeDto;
        }

        public async Task<IEnumerable<EmployeeDto>> GetEmployeesAsync(Guid companyId, bool trackChanges)
        {
            var company = await repository.Company.GetCompanyAsync(companyId, trackChanges);

            if (company == null)
            {
                throw new CompanyNotFoundException(companyId);
            }

            var emploees = await repository.Employee.GetEmployeesAsync(companyId, trackChanges);

            var emploeesDto = _mapper.Map<IEnumerable<EmployeeDto>>(emploees);

            return emploeesDto;
        }

        public async Task UpdateEmployeeForCompanyAsync(Guid companyId, Guid id, EmployeeForUpdateDto employee, bool companyTrackChanges, bool employeeTrackChanges)
        {
            var company = await repository.Company.GetCompanyAsync(companyId, companyTrackChanges);

            if(company is null)
            {
                throw new CompanyNotFoundException(companyId);
            }

            var employeeEntity = await repository.Employee.GetEmployeeAsync(companyId, id, employeeTrackChanges);

            if (employeeEntity is null) 
            {
                throw new EmployeeNotFoundException(id);
            }

            _mapper.Map(employee, employeeEntity);

            await repository.SaveAsync();
        }
        public async Task<(EmployeeForUpdateDto employeeToPatch, Employee employeeEntity)> GetEmployeeForPatchAsync(Guid companyId, Guid id, bool compTrackChanges, bool empTrackChanges)
        {
            var company = await repository.Company.GetCompanyAsync(companyId, compTrackChanges);

            if (company is null)
                throw new CompanyNotFoundException(companyId);

            var employeeEntity = await repository.Employee.GetEmployeeAsync(companyId, id, empTrackChanges);
            
            if (employeeEntity is null)
                throw new EmployeeNotFoundException(companyId);
            
            var employeeToPatch = _mapper.Map<EmployeeForUpdateDto>(employeeEntity);
            
            return (employeeToPatch, employeeEntity);
        }
        public async Task SaveChangesForPatchAsync(EmployeeForUpdateDto employeeToPatch, Employee employeeEntity)
        {
            _mapper.Map(employeeToPatch, employeeEntity);
            
            await repository.SaveAsync();
        }
    }
}
