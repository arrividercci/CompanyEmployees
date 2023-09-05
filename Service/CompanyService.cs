using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    internal sealed class CompanyService : ICompanyService
    {
        private readonly IRepositoryManager repository;
        private readonly ILoggerManager logger;
        private readonly IMapper _mapper;
        public CompanyService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
        {
            this.repository = repository;
            this.logger = logger;
            this._mapper = mapper;
        }
        private async Task<Company> GetCompanyAndCheckIfItExists(Guid id, bool trackChanges)
        {
            var company = await repository.Company.GetCompanyAsync(id, trackChanges);

            if (company is null)
            {
                throw new CompanyNotFoundException(id);
            }

            return company;
        }

        public async Task<CompanyDto> CreateCompanyAsync(CompanyForCreationDto company)
        {
            var companyEntity = _mapper.Map<Company>(company);

            repository.Company.CreateCompany(companyEntity);

            await repository.SaveAsync();

            var companyToReturn = _mapper.Map<CompanyDto>(companyEntity);

            return companyToReturn;
        }

        public async Task<(IEnumerable<CompanyDto> companies, string ids)> CreateCompanyCollectionAsync(IEnumerable<CompanyForCreationDto> companyCollection)
        {
            if (companyCollection is null) 
            {
                throw new CompanyCollectionBadRequest();
            }

            var companyEntities = _mapper.Map<IEnumerable<Company>>(companyCollection);

            foreach (var companyEntity in companyEntities) 
            {
                repository.Company.CreateCompany(companyEntity);
            }

            await repository.SaveAsync();

            var companyCollectionToReturn = _mapper.Map<IEnumerable<CompanyDto>>(companyEntities);

            var ids = string.Join(",", companyCollectionToReturn.Select(company => company.Id));

            return (companies: companyCollectionToReturn, ids);
        }

        public async Task DeleteCompanyAsync(Guid companyId, bool trackChanges)
        {
            var company = await GetCompanyAndCheckIfItExists(companyId, trackChanges);

            repository.Company.DeleteCompany(company);

            await repository.SaveAsync();
        }

        public async Task<IEnumerable<CompanyDto>> GetAllCompaniesAsync(bool trackChanges)
        {
           
            var companies = await repository.Company.GetAllCompaniesAsync(trackChanges);
                
            var companiesDto = _mapper.Map<IEnumerable<CompanyDto>>(companies);

            return companiesDto;
            
        }

        public async Task<IEnumerable<CompanyDto>> GetByIdsAsync(IEnumerable<Guid> ids, bool trackChanges)
        {
            if (ids is null)
            {
                throw new CollectionByIdsBadRequestException();
            }

            var companiesEntities = await repository.Company.GetByIdsAsync(ids, trackChanges);

            if (ids.Count() != companiesEntities.Count()) 
            {
                throw new CollectionByIdsBadRequestException();
            }

            var companiesToReturn = _mapper.Map<IEnumerable<CompanyDto>>(companiesEntities);

            return companiesToReturn;
        }

        public async Task<CompanyDto> GetCompanyAsync(Guid companyId, bool trackChanges)
        {
            var company = await GetCompanyAndCheckIfItExists(companyId, trackChanges);

            var companyDto = _mapper.Map<CompanyDto>(company);

            return companyDto;
        }

        public async Task UpdateCompanyAsync(Guid companyId, CompanyForUpdateDto company, bool trackChanges)
        {
            var companyEntity = await GetCompanyAndCheckIfItExists(companyId, trackChanges);

            _mapper.Map(company, companyEntity);

            await repository.SaveAsync();
        }
    }
}
