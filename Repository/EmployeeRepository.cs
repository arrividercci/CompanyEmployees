using Contracts;
using Entities.Models;
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

        public Employee GetEmployee(Guid companyId, Guid emploeeId, bool trackChanges)
        {
            return FindByCondition(employee => employee.Id.Equals(emploeeId) && employee.CompanyId.Equals(companyId), trackChanges)
                .SingleOrDefault();
        }

        public IEnumerable<Employee> GetEmployees(Guid companyId, bool trackChanges)
        {
            return FindByCondition(emploee => emploee.CompanyId.Equals(companyId), trackChanges)
                   .OrderBy(employee => employee.Name)
                   .ToList();
        }
    }
}
