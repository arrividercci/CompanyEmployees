using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Extensions
{
    public static class RepositoryEmployeeExtensions
    {
        public static IQueryable<Employee> FilterEmployees(this IQueryable<Employee> employees, uint minAge, uint maxAge) 
        {
            return employees.Where(employee => employee.Age >= minAge && employee.Age <= maxAge);
        }

        public static IQueryable<Employee> Search(this IQueryable<Employee> employees, string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return employees;
            } else
            {
                var lowerCaseSearchTerm = searchTerm.Trim().ToLower();

                return employees.Where(employee => employee.Name.ToLower().Contains(lowerCaseSearchTerm));
            }
        }
    }
}
