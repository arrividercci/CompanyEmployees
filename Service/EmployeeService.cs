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

        public EmployeeDto CreateEmployeeForCompany(Guid companyId, EmployeeForCreationDto employee, bool trackChanges)
        {
            var company = repository.Company.GetCompany(companyId, trackChanges);

            if (company is null)
            {
                throw new CompanyNotFoundException(companyId);
            }

            var employeeEntity = _mapper.Map<Employee>(company);

            repository.Employee.CreateEmployeeForCompany(companyId, employeeEntity);

            repository.Save();

            var employeeToReturn = _mapper.Map<EmployeeDto>(employee);

            return employeeToReturn;
        }

        public void DeleteEmployeeForCompany(Guid companyId, Guid id, bool trackChanges)
        {
            var company = repository.Company.GetCompany(companyId, trackChanges);

            if (company is null)
            {
                throw new CompanyNotFoundException(companyId);
            }

            var employeeForCompany = repository.Employee.GetEmployee(companyId, id, trackChanges);

            if (employeeForCompany is null)
            {
                throw new EmployeeNotFoundException(id);
            }

            repository.Employee.DeleteEmployee(employeeForCompany);
            
            repository.Save();
        }

        public EmployeeDto GetEmployee(Guid companyId, Guid id, bool trackChanges)
        {
            var company = repository.Company.GetCompany(companyId, trackChanges);

            if (company == null)
            {
                throw new CompanyNotFoundException(companyId);
            }

            var emploee = repository.Employee.GetEmployee(companyId, id, trackChanges);

            if (emploee == null)
            {
                throw new EmployeeNotFoundException(id);
            }

            var employeeDto = _mapper.Map<EmployeeDto>(emploee);

            return employeeDto;
        }

        public IEnumerable<EmployeeDto> GetEmployees(Guid companyId, bool trackChanges)
        {
            var company = repository.Company.GetCompany(companyId, trackChanges);

            if (company == null)
            {
                throw new CompanyNotFoundException(companyId);
            }

            var emploees = repository.Employee.GetEmployees(companyId, trackChanges);

            var emploeesDto = _mapper.Map<IEnumerable<EmployeeDto>>(emploees);

            return emploeesDto;
        }

        public void UpdateEmployeeForCompany(Guid companyId, Guid id, EmployeeForUpdateDto employee, bool companyTrackChanges, bool employeeTrackChanges)
        {
            var company = repository.Company.GetCompany(companyId, companyTrackChanges);

            if(company is null)
            {
                throw new CompanyNotFoundException(companyId);
            }

            var employeeEntity = repository.Employee.GetEmployee(companyId, id, employeeTrackChanges);

            if (employeeEntity is null) 
            {
                throw new EmployeeNotFoundException(id);
            }

            _mapper.Map(employee, employeeEntity);

            repository.Save();
        }
        public (EmployeeForUpdateDto employeeToPatch, Employee employeeEntity) GetEmployeeForPatch(Guid companyId, Guid id, bool compTrackChanges, bool empTrackChanges)
        {
            var company = repository.Company.GetCompany(companyId, compTrackChanges);

            if (company is null)
                throw new CompanyNotFoundException(companyId);

            var employeeEntity = repository.Employee.GetEmployee(companyId, id, empTrackChanges);
            if (employeeEntity is null)
                throw new EmployeeNotFoundException(companyId);
            
            var employeeToPatch = _mapper.Map<EmployeeForUpdateDto>(employeeEntity);
            
            return (employeeToPatch, employeeEntity);
        }
        public void SaveChangesForPatch(EmployeeForUpdateDto employeeToPatch, Employee employeeEntity)
        {
            _mapper.Map(employeeToPatch, employeeEntity);
            
            repository.Save();
        }
    }
}
