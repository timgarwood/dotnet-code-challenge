using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CodeChallenge.Services;
using CodeChallenge.Models;
using System.Net;
using CodeChallenge.Exceptions;

namespace CodeChallenge.Controllers
{
    [ApiController]
    [Route("api/employee")]
    public class EmployeeController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IEmployeeService _employeeService;

        public EmployeeController(ILogger<EmployeeController> logger, IEmployeeService employeeService)
        {
            _logger = logger;
            _employeeService = employeeService;
        }

        [HttpPost]
        public IActionResult CreateEmployee([FromBody] Employee employee)
        {
            _logger.LogDebug($"Received employee create request for '{employee.FirstName} {employee.LastName}'");

            _employeeService.Create(employee);

            return CreatedAtRoute("getEmployeeById", new { id = employee.EmployeeId }, employee);
        }

        [HttpGet("{id}", Name = "getEmployeeById")]
        public IActionResult GetEmployeeById(String id)
        {
            _logger.LogDebug($"Received employee get request for '{id}'");

            var employee = _employeeService.GetById(id);

            if (employee == null)
                return NotFound();

            return Ok(employee);
        }

        /// <summary>
        /// Endpoint to return the reporting structure of the given employee
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("structure/{id}")]
        [HttpGet]
        public IActionResult GetReportingStructureById(String id)
        {
            _logger.LogDebug($"Received reporting structure get request for '{id}'");

            var structure = _employeeService.GetReportingStructureById(id);

            if(structure == null)
            {
                return NotFound();
            }

            return Ok(structure);
        }

        [HttpPut("{id}")]
        public IActionResult ReplaceEmployee(String id, [FromBody]Employee newEmployee)
        {
            _logger.LogDebug($"Recieved employee update request for '{id}'");

            var existingEmployee = _employeeService.GetById(id);
            if (existingEmployee == null)
                return NotFound();

            _employeeService.Replace(existingEmployee, newEmployee);

            return Ok(newEmployee);
        }

        /// <summary>
        /// Retrieve a Compensation by employee id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("compensation/{id}")]
        public IActionResult ReadById(string id)
        {
            _logger.LogDebug($"received compensation get request for employee {id}");

            var compensation = _employeeService.GetCompensationById(id);

            if (compensation == null)
            {
                return NotFound();
            }

            return Ok(compensation);
        }

        /// <summary>
        /// Create a new Compensation for an Employee
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("compensation")]
        public IActionResult Create([FromBody] CompensationCreateModel model)
        {
            _logger.LogDebug($"received compensation create request for employee {model.EmployeeId}");

            try
            {
                var compensation = _employeeService.Create(model);

                if(compensation == null)
                {
                    return NotFound();
                }

                return Ok(compensation);
            }
            catch(CompensationAlreadyExistsException)
            {
                _logger.LogError($"Compensation already exists for employee {model.EmployeeId}");
                // for now, just return conflict.
                // in a real API, we might put an exception status code
                // in the response object
                return Conflict();
            }
        }
    }
}
