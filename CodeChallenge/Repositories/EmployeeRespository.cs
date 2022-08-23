using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeChallenge.Models;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using CodeChallenge.Data;

namespace CodeChallenge.Repositories
{
    public class EmployeeRespository : IEmployeeRepository
    {
        private readonly EmployeeContext _employeeContext;
        private readonly ILogger<IEmployeeRepository> _logger;

        public EmployeeRespository(ILogger<IEmployeeRepository> logger, EmployeeContext employeeContext)
        {
            _employeeContext = employeeContext;
            _logger = logger;
        }

        public Employee Add(Employee employee)
        {
            employee.EmployeeId = Guid.NewGuid().ToString();
            _employeeContext.Employees.Add(employee);
            return employee;
        }

        public Employee GetById(string id)
        {
            return _employeeContext.Employees
                .Include(e => e.Compensation)
                .SingleOrDefault(e => e.EmployeeId == id);
        }

        /// <summary>
        /// Retrieve the given employee including their direct reports
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Employee GetByIdWithDirectReports(string id)
        {
            return _employeeContext.Employees
                .Include(e => e.Compensation)
                .Include(e => e.DirectReports)
                .SingleOrDefault(e => e.EmployeeId == id);
        }

        /// <summary>
        /// Retrieve the Compensation for the given employee
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Compensation GetCompensationById(string id)
        {
            var employee = _employeeContext.Employees
                .Include(e => e.Compensation)
                .SingleOrDefault(e => e.EmployeeId == id);

            return employee?.Compensation;
        }

        /// <summary>
        /// Create a new compensation
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Compensation Create(CompensationCreateModel model)
        {
            var employee = _employeeContext.Employees
                .SingleOrDefault(e => e.EmployeeId == model.EmployeeId);

            var compensation = new Compensation
            {
                Id = Guid.NewGuid().ToString(),
                Salary = model.Salary,
                EffectiveDate = model.EffectiveDate,
                EmployeeId = model.EmployeeId
            };

            _employeeContext.Compensations.Add(compensation);

            return compensation;
        }

        public Task SaveAsync()
        {
            return _employeeContext.SaveChangesAsync();
        }

        public Employee Remove(Employee employee)
        {
            return _employeeContext.Remove(employee).Entity;
        }
    }
}
