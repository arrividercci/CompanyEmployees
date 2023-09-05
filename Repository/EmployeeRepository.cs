using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Repository.Extensions;
using Shared.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class EmployeeRepository : RepositoryBase<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(RepositoryContext context) : base(context)
        {
        }

        public void CreateEmployeeForCompany(Guid companyId, Employee employee)
        {
            employee.CompanyId = companyId;
            Create(employee);
        }

        public void DeleteEmployee(Employee employee)
        {
            Delete(employee);
        }

        public async Task<Employee> GetEmployeeAsync(Guid companyId, Guid emploeeId, bool trackChanges)
        {
            return await FindByCondition(employee => employee.Id.Equals(emploeeId) && employee.CompanyId.Equals(companyId), trackChanges)
                .SingleOrDefaultAsync();
        }

        public async Task<PagedList<Employee>> GetEmployeesAsync(Guid companyId, EmployeeParameters employeeParameters, bool trackChanges)
        {
            var employees =  await FindByCondition(emploee => (emploee.CompanyId.Equals(companyId)), trackChanges)
                                   .FilterEmployees(employeeParameters.MinAge, employeeParameters.MaxAge)
                                   .Search(employeeParameters.SearchTerm)
                                   .OrderBy(employee => employee.Name)
                                   .ToListAsync();

            return PagedList<Employee>.ToPagedList(employees, employeeParameters.PageNumber, employeeParameters.PageSize);
        }
    }
}
