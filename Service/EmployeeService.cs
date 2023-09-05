using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
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

        private async Task CheckIfCompanyExists(Guid id, bool trackChanges)
        {
            var company = await repository.Company.GetCompanyAsync(id, trackChanges);

            if (company is null)
            {
                throw new CompanyNotFoundException(id);
            }
        }

        private async Task<Employee> GetEmployeeForCompanyAndCheckIfItExists(Guid companyId, Guid id, bool trackChanges)
        {
            var employeeDb = await repository.Employee.GetEmployeeAsync(companyId, id, trackChanges);
            if (employeeDb is null) throw new EmployeeNotFoundException(id);
            return employeeDb;
        }


        public async Task<EmployeeDto> CreateEmployeeForCompanyAsync(Guid companyId, EmployeeForCreationDto employee, bool trackChanges)
        {
            await CheckIfCompanyExists(companyId, trackChanges);

            var employeeEntity = _mapper.Map<Employee>(employee);

            repository.Employee.CreateEmployeeForCompany(companyId, employeeEntity);

            await repository.SaveAsync();

            var employeeToReturn = _mapper.Map<EmployeeDto>(employee);

            return employeeToReturn;
        }

        public async Task DeleteEmployeeForCompanyAsync(Guid companyId, Guid id, bool trackChanges)
        {
            await CheckIfCompanyExists(companyId, trackChanges);

            var employeeForCompany = await GetEmployeeForCompanyAndCheckIfItExists(companyId, id, trackChanges);

            repository.Employee.DeleteEmployee(employeeForCompany);
            
            await repository.SaveAsync();
        }

        public async Task<EmployeeDto> GetEmployeeAsync(Guid companyId, Guid id, bool trackChanges)
        {
            await CheckIfCompanyExists(companyId, trackChanges);

            var emploee = await repository.Employee.GetEmployeeAsync(companyId, id, trackChanges);

            if (emploee == null)
            {
                throw new EmployeeNotFoundException(id);
            }

            var employeeDto = _mapper.Map<EmployeeDto>(emploee);

            return employeeDto;
        }

        public async Task<(IEnumerable<EmployeeDto> employees, MetaData metaData)> GetEmployeesAsync(Guid companyId, EmployeeParameters employeeParameters, bool trackChanges)
        {
            if (!employeeParameters.ValidAgeRange)
                throw new MaxAgeRangeBadRequestException();

            await CheckIfCompanyExists(companyId, trackChanges);
            
            var emploeesWithMetaData = await repository.Employee.GetEmployeesAsync(companyId, employeeParameters, trackChanges);

            var emploeesDto = _mapper.Map<IEnumerable<EmployeeDto>>(emploeesWithMetaData);

            return (emploees: emploeesDto, metaData: emploeesWithMetaData.MetaData);
        }

        public async Task UpdateEmployeeForCompanyAsync(Guid companyId, Guid id, EmployeeForUpdateDto employee, bool companyTrackChanges, bool employeeTrackChanges)
        {
            await CheckIfCompanyExists(companyId, companyTrackChanges);

            var employeeEntity = await GetEmployeeForCompanyAndCheckIfItExists(companyId, id, employeeTrackChanges);

            _mapper.Map(employee, employeeEntity);

            await repository.SaveAsync();
        }
        public async Task<(EmployeeForUpdateDto employeeToPatch, Employee employeeEntity)> GetEmployeeForPatchAsync(Guid companyId, Guid id, bool compTrackChanges, bool empTrackChanges)
        {
            await CheckIfCompanyExists(companyId, compTrackChanges);

            var employeeEntity = await GetEmployeeForCompanyAndCheckIfItExists(companyId, id, empTrackChanges);
            
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
