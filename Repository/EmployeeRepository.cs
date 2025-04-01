using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Shared.RequestFeatures;
using Repository.Extentions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class EmployeeRepository(RepositoryContext repositoryContext) : RepositoryBase<Employee>(repositoryContext), IEmployeeRepository
    {
        public async Task<PagedList<Employee>> GetEmployeesAsync(Guid companyId,
EmployeeParameters employeeParameters, bool trackChanges)
        {


            var employees = await FindByCondition(e => e.CompanyId.Equals(companyId),
            trackChanges)
            .FilterEmployees(employeeParameters.MinAge, employeeParameters.MaxAge)
            .Search(employeeParameters.SearchTerm)
            .Sort(employeeParameters.OrderBy)
            .ToListAsync();
            return PagedList<Employee>
            .ToPagedList(employees, employeeParameters.PageNumber,
            employeeParameters.PageSize);
        }

        public async Task<Employee> GetEmployeeAsync(Guid companyId, Guid id, bool trackChanges) => await FindByCondition(e => e.CompanyId.Equals(companyId) && e.Id.Equals(id), trackChanges)
            .SingleOrDefaultAsync();

        public void CreateEmployeeForCompany(Guid companyId, Employee employee)
        {
            employee.CompanyId = companyId;
            Create(employee);
        }

        public async Task<IEnumerable<Employee>> GetByIdsAsync(IEnumerable<Guid> ids, bool trackChanges) => await FindByCondition(e => ids.Contains(e.Id), trackChanges)
            .ToListAsync();

        public void DeleteEmployee(Employee employee) => Delete(employee);
    }
}
