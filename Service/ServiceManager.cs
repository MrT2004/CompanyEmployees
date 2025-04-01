using Contracts;
using Service.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Service.DataShaping;
using Shared.DataTransferObjects;

namespace Service
{
    public sealed class ServiceManager(IRepositoryManager repositoryManager, ILoggerManager logger,
IMapper mapper, IEmployeeLinks employeeLinks) : IServiceManager
    {
        private readonly Lazy<ICompanyService> _companyService = new(() =>
            new CompanyService(repositoryManager, logger, mapper));
        private readonly Lazy<IEmployeeService> _employeeService = new(() =>
            new EmployeeService(repositoryManager, logger, mapper, employeeLinks));

        public ICompanyService CompanyService => _companyService.Value;
        public IEmployeeService EmployeeService => _employeeService.Value;
    }
}
