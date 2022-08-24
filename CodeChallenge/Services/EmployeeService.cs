using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeChallenge.Models;
using Microsoft.Extensions.Logging;
using CodeChallenge.Repositories;
using CodeChallenge.Exceptions;

namespace CodeChallenge.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILogger<EmployeeService> _logger;

        public EmployeeService(ILogger<EmployeeService> logger, IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
            _logger = logger;
        }

        public Employee Create(Employee employee)
        {
            if(employee != null)
            {
                _employeeRepository.Add(employee);
                _employeeRepository.SaveAsync().Wait();
            }

            return employee;
        }

        public Employee GetById(string id)
        {
            if(!String.IsNullOrEmpty(id))
            {
                return _employeeRepository.GetById(id);
            }

            return null;
        }

        public Employee Replace(Employee originalEmployee, Employee newEmployee)
        {
            if(originalEmployee != null)
            {
                _employeeRepository.Remove(originalEmployee);
                if (newEmployee != null)
                {
                    // ensure the original has been removed, otherwise EF will complain another entity w/ same id already exists
                    _employeeRepository.SaveAsync().Wait();

                    _employeeRepository.Add(newEmployee);
                    // overwrite the new id with previous employee id
                    newEmployee.EmployeeId = originalEmployee.EmployeeId;
                }
                _employeeRepository.SaveAsync().Wait();
            }

            return newEmployee;
        }

        /// <summary>
        /// Return the reporting structure for the given employee
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ReportingStructureModel GetReportingStructureById(String id)
        {
            if (String.IsNullOrEmpty(id)) return null;

            var topEmployee = _employeeRepository.GetByIdWithDirectReports(id);

            if (topEmployee == null) return null;

            return new ReportingStructureModel
            {
                Employee = topEmployee,
                NumberOfReports = GetNumberOfReports(topEmployee)
            };
        }

        /// <summary>
        /// Retrieve the Compensation for the given Employee
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Compensation GetCompensationById(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;

            var compensation = _employeeRepository.GetCompensationById(id);

            return compensation;
        }

        /// <summary>
        /// Create a new Compensation
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Compensation Create(CompensationCreateModel model)
        {
            if (model == null) return null;

            var employee = _employeeRepository.GetById(model.EmployeeId);

            if (employee == null) return null;

            if (employee.Compensation != null) throw new CompensationAlreadyExistsException();

            var compensation = _employeeRepository.Create(model);

            _employeeRepository.SaveAsync().Wait();

            return compensation;
        }

        /// <summary>
        /// Recursive method to determine the number of employees
        /// below the root employee in the organization
        /// </summary>
        /// <param name="rootEmployee"></param>
        /// <returns></returns>
        private int GetNumberOfReports(Employee rootEmployee)
        {
            if (rootEmployee.DirectReports == null) return 0;

            var sum = 0;

            // this is pretty slow because it creates many database queries
            // an alternative is to create a stored procedure to do this at the
            // database level
            foreach (var directReport in rootEmployee.DirectReports)
            {
                var directReportWithReports = _employeeRepository.GetByIdWithDirectReports(directReport.EmployeeId);
                
                sum += GetNumberOfReports(directReportWithReports);
            }

            return sum + rootEmployee.DirectReports.Count;
        }
    }
}
