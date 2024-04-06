using Entities.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Shared.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Tests
{
    public class CompanyRepositoryTests
    {
        private RepositoryContext _context;
        private CompanyRepository _companyRepository;

        [SetUp]
        public void Setup()
        {
            var builder = new DbContextOptionsBuilder<RepositoryContext>().UseInMemoryDatabase($"RepositoryContext-{Guid.NewGuid()}");
            _context = new RepositoryContext(builder.Options);
            _companyRepository = new CompanyRepository(_context);
        }

        [Test]
        public async Task CreateCompany_Creates_Company()
        {
            // Arrange
            var previousCompanyCount = _context.Companies.Count();
            var company = new Company
            {
                Id = Guid.NewGuid(),
                Name = "Company1",
                Country = "Location1",
                Address = "Address1"
            };

            // Act
            _companyRepository.CreateCompany(company);
            await _companyRepository.SaveAsync();

            // Assert
            Assert.That(_context.Companies, Has.Member(company));
            Assert.That(_context.Companies.Count(), Is.EqualTo(previousCompanyCount + 1));
        }

        [Test]
        public async Task DeleteCompany_Removes_Company_From_Database()
        {
            // Arrange
            var company = new Company
            {
                Id = Guid.NewGuid(),
                Name = "Company2",
                Country = "Location2",
                Address = "Address1"

            };
            _context.Companies.Add(company);
            _context.SaveChanges();
            var previousCompanyCount = _context.Companies.Count();

            // Act
            _companyRepository.DeleteCompany(company);
            await _companyRepository.SaveAsync();

            // Assert
            Assert.That(_context.Companies, Has.No.Member(company));
            Assert.That(_context.Companies.Count(), Is.EqualTo(previousCompanyCount - 1));
        }

        [Test]
        public async Task GetAllCompaniesAsync_Returns_All_Companies()
        {
            // Arrange
            var company1 = new Company { Id = Guid.NewGuid(), Name = "Company1", Country = "Location1", Address = "Address1" };
            var company2 = new Company { Id = Guid.NewGuid(), Name = "Company2", Country = "Location2", Address = "Address2" };
            var company3 = new Company { Id = Guid.NewGuid(), Name = "Company3", Country = "Location3", Address = "Address1" };
            _context.Companies.AddRange(new[] { company1, company2, company3 });
            await _companyRepository.SaveAsync();
            var count = _context.Companies.Count();

            // Act
            var result = await _companyRepository.GetAllCompaniesAsync(false);

            // Assert
            Assert.That(result.Count(), Is.EqualTo(count));
        }

        [Test]
        public async Task GetByIdsAsync_Returns_Correct_Companies()
        {
            // Arrange
            var companyId1 = Guid.NewGuid();
            var companyId2 = Guid.NewGuid();
            var companyId3 = Guid.NewGuid();
            var company1 = new Company { Id = companyId1, Name = "Company1", Country = "Location1", Address = "Address1" };
            var company2 = new Company { Id = companyId2, Name = "Company2", Country = "Location2", Address = "Address2" };
            var company3 = new Company { Id = companyId3, Name = "Company3", Country = "Location3", Address = "Address1" };
            _context.Companies.AddRange(new[] { company1, company2, company3 });
            await _companyRepository.SaveAsync();
            var ids = new List<Guid> { companyId1, companyId3 };

            // Act
            var result = await _companyRepository.GetByIdsAsync(ids, false);

            // Assert
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.Select(x => x.Id), Has.Member(company1.Id));
            Assert.That(result.Select(x => x.Id), Has.Member(company3.Id));
            Assert.That(result.Select(x => x.Id), Has.No.Member(company2.Id));
        }

        [Test]
        public async Task GetCompanyAsync_Returns_Correct_Company()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var company = new Company { Id = companyId, Name = "Company1", Country = "Location1" , Address = "Address1" };
            _context.Companies.Add(company);
            _context.SaveChanges();

            // Act
            var result = await _companyRepository.GetCompanyAsync(companyId, false);

            // Assert
            Assert.That(result.Id, Is.EqualTo(company.Id));
            Assert.That(result.Name, Is.EqualTo("Company1"));
            Assert.That(result.Country, Is.EqualTo("Location1"));
        }
    }

}
