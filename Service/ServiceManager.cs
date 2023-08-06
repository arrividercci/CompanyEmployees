﻿using Contracts;
using Service.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public sealed class ServiceManager : IServiceManager
    {
        private readonly Lazy<ICompanyService> companyService;
        private readonly Lazy<IEmploeeService> employeeService;
        public ServiceManager(IRepositoryManager repositoryManager, ILoggerManager logger)
        {
            companyService = new Lazy<ICompanyService>(() => new CompanyService(repositoryManager, logger));
            employeeService = new Lazy<IEmploeeService>(() => new EmployeeService(repositoryManager, logger));
        }

        public ICompanyService CompanyService => companyService.Value;

        public IEmploeeService EmploeeService => employeeService.Value;
    }
}