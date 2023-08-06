using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public sealed class RepositoryManager : IRepositoryManager
    {
        private readonly RepositoryContext context;
        private readonly Lazy<ICompanyRepository> companyRepository;
        private readonly Lazy<IEmployeeRepository> employeeRepository;
        public RepositoryManager(RepositoryContext context)
        {
            this.context = context;
            companyRepository = new Lazy<ICompanyRepository>(() => new CompanyRepository(this.context));
            employeeRepository = new Lazy<IEmployeeRepository>(() => new EmployeeRepository(this.context));
        }
        public ICompanyRepository Company => companyRepository.Value;

        public IEmployeeRepository Employee => employeeRepository.Value;

        public void Save() => context.SaveChanges();
    }
}
