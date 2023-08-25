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
    public class CompanyRepository : RepositoryBase<Company>, ICompanyRepository
    {
        public CompanyRepository(RepositoryContext context) : base(context)
        {
        }

        public void CreateCompany(Company company)
        {
            Create(company);
        }

        public void DeleteCompany(Company company)
        {
            Delete(company);
        }

        public async Task<IEnumerable<Company>> GetAllCompaniesAsync(bool trackChanges)
        {
            return await FindAll(trackChanges)
                .OrderBy(company => company.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Company>> GetByIdsAsync(IEnumerable<Guid> ids, bool trackChanges)
        {
            return await FindByCondition(company => ids.Contains(company.Id), trackChanges)
                .ToListAsync();
        }

        public async Task<Company> GetCompanyAsync(Guid companyId, bool trackChanges)
        {
            return await FindByCondition(company => company.Id.Equals(companyId), trackChanges)
                .SingleOrDefaultAsync();
        }
    }
}
