using Entities.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Shared.RequestFeatures;

namespace Repository.Tests
{
    public class EmploeesRepositoryTests
    {
        private RepositoryContext _context;
        private EmployeeRepository _employeeRepository;

        [SetUp]
        public void Setup()
        {  
            var builder = new DbContextOptionsBuilder<RepositoryContext>().UseInMemoryDatabase($"RepositoryContext-{Guid.NewGuid()}");
            _context =  new RepositoryContext(builder.Options);
            _employeeRepository = new EmployeeRepository(_context);

        }

        [Test]
        public async Task CreateEmployeeForCompany_Creates_Employee_For_Company()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var previousEmploeeCount = _context.Employees.Count();
            var employee = new Employee 
            { 
                Id = Guid.NewGuid(), 
                Name = "Alex",
                Position = "Manager",
                CompanyId = companyId
            };

            // Act
            _employeeRepository.CreateEmployeeForCompany(companyId, employee);
            await _employeeRepository.SaveAsync();

            // Assert
            Assert.That(_context.Employees, Has.Member(employee));
            Assert.That(_context.Employees, Is.Unique);
        }

        [Test]
        public async Task CreateEmployeeForCompany_Throws_Exception_If_Position_Not_Set()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var employee = new Employee
            {
                Id = Guid.NewGuid(),
                Name = "Alex",
                //Position = "Manager",
                CompanyId = companyId
            };

            // Act
            var act = async () =>
            {
                _employeeRepository.CreateEmployeeForCompany(companyId, employee);
                await _employeeRepository.SaveAsync();
            };

            // Assert
            await act.Should().ThrowAsync<Exception>();
        }

        [Test]
        public async Task DeleteEmployee_Removes_Employee_From_Database()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var employee = new Employee
            {
                Id = Guid.NewGuid(),
                Name = "John",
                Position = "Developer",
                CompanyId = companyId
            };
            _context.Employees.Add(employee);
            _context.SaveChanges();
            var previousEmploeeCount = _context.Employees.Count();

            // Act
            _employeeRepository.DeleteEmployee(employee);
            await _employeeRepository.SaveAsync();

            // Assert
            Assert.That(_context.Employees, Has.No.Member(employee));
            Assert.That(_context.Employees.Count(), Is.EqualTo(previousEmploeeCount - 1));
        }

        [Test]
        public async Task GetEmployeeAsync_Returns_Correct_Employee()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var employeeId = Guid.NewGuid();
            var employee = new Employee
            {
                Id = employeeId,
                Name = "Jane",
                Position = "HR",
                CompanyId = companyId
            };
            _employeeRepository.CreateEmployeeForCompany(companyId, employee);
            await _employeeRepository.SaveAsync();

            // Act
            var result = await _employeeRepository.GetEmployeeAsync(companyId, employeeId, false);

            // Assert
            Assert.That(result.Id, Is.EqualTo(employee.Id));
            Assert.That(result.Name, Is.EqualTo("Jane"));
            Assert.That(result.Position, Is.EqualTo("HR"));
            Assert.That(result.CompanyId, Is.EqualTo(companyId));
        }

        [Test]
        public async Task GetEmployeesAsync_Returns_PagedList()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var employee1 = new Employee { Id = Guid.NewGuid(), Name = "John", Position = "Developer", CompanyId = companyId };
            var employee2 = new Employee { Id = Guid.NewGuid(), Name = "Jane", Position = "HR", CompanyId = companyId };
            var employee3 = new Employee { Id = Guid.NewGuid(), Name = "Alex", Position = "Manager", CompanyId = companyId };
            _context.Employees.AddRange(new[] { employee1, employee2, employee3 });
            await _employeeRepository.SaveAsync();
            var parameters = new EmployeeParameters { PageNumber = 1, PageSize = 2 };

            // Act
            var result = await _employeeRepository.GetEmployeesAsync(companyId, parameters, false);

            // Assert
            Assert.That(result.Count, Is.EqualTo(2));
        }

        [Test]
        [TestCase(1, 2, 2)]
        [TestCase(1, 3, 3)]
        [TestCase(2, 2, 1)]
        public async Task GetEmployeesAsync_Returns_Correct_Counted_PagedList(int pageNumber, int pageSize, int expectedCount)
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var employee1 = new Employee { Id = Guid.NewGuid(), Name = "John", Position = "Developer", CompanyId = companyId };
            var employee2 = new Employee { Id = Guid.NewGuid(), Name = "Jane", Position = "HR", CompanyId = companyId };
            var employee3 = new Employee { Id = Guid.NewGuid(), Name = "Alex", Position = "Manager", CompanyId = companyId };
            _context.Employees.AddRange(new[] { employee1, employee2, employee3 });
            await _employeeRepository.SaveAsync();

            var parameters = new EmployeeParameters { PageNumber = pageNumber, PageSize = pageSize };

            // Act
            var result = await _employeeRepository.GetEmployeesAsync(companyId, parameters, false);

            // Assert
            Assert.That(result.Count, Is.EqualTo(expectedCount));
        }


        [Test]
        [TestCase("Some")]
        [TestCase("Alex")]
        [TestCase("A")]
        public async Task GetEmployeesAsync_By_SearchTerm_ReturnsCorrectPagedList(string searchTerm)
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var employee1 = new Employee { Id = Guid.NewGuid(), Name = "John", Position = "Developer", CompanyId = companyId };
            var employee2 = new Employee { Id = Guid.NewGuid(), Name = "Jane", Position = "HR", CompanyId = companyId };
            var employee3 = new Employee { Id = Guid.NewGuid(), Name = "Alex", Position = "Manager", CompanyId = companyId };
            _context.Employees.AddRange(new[] { employee1, employee2, employee3 });
            await _employeeRepository.SaveAsync();

            var parameters = new EmployeeParameters {SearchTerm = searchTerm};

            // Act
            var result = await _employeeRepository.GetEmployeesAsync(companyId, parameters, false);

            // Assert
            Assert.That(result.Any() ? result.All(x => x.Name.Contains(searchTerm, StringComparison.InvariantCultureIgnoreCase)) : true);
        }
    }
}