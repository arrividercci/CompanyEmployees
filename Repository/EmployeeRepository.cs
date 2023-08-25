using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
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

        public async Task<IEnumerable<Employee>> GetEmployeesAsync(Guid companyId, bool trackChanges)
        {
            return await FindByCondition(emploee => emploee.CompanyId.Equals(companyId), trackChanges)
                   .OrderBy(employee => employee.Name)
                   .ToListAsync();
        }
    }
}
