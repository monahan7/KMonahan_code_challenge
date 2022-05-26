using System;
using challenge.Models;
using challenge.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace challenge.Controllers
{
	[Route("api/employee")]
	public class EmployeeController : Controller
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

		[HttpPut("{id}")]
		public IActionResult ReplaceEmployee(String id, [FromBody] Employee newEmployee)
		{
			_logger.LogDebug($"Recieved employee update request for '{id}'");

			var existingEmployee = _employeeService.GetById(id);
			if (existingEmployee == null)
				return NotFound();

			_employeeService.Replace(existingEmployee, newEmployee);

			return Ok(newEmployee);
		}

		#region KMONAHAN: ReportingStructure
		[HttpGet("GetReportingStructure/{id}")]
		public IActionResult GetReportingStructure(String id)
		{
			_logger.LogDebug($"Received employee get request for '{id}'");

			var reportingStructure = _employeeService.GetReportingStructure(id);

			if (reportingStructure == null)
				return NotFound();

			return Ok(reportingStructure);
		}
		#endregion

		#region KMONAHAN: Compensation
		[HttpPost("CreateCompensation")]
		public IActionResult CreateCompensation([FromBody] Compensation compensation)
		{
			_logger.LogDebug($"Received compensation create request for '{compensation.employee.FirstName} {compensation.employee.LastName}'");

			_employeeService.CreateCompensation(compensation);

			return CreatedAtRoute("getCompensationByEmployeeId", new { id = compensation.CompensationId }, compensation);
		}

		[HttpGet("GetCompensation/{id}", Name = "getCompensationByEmployeeId")]
		public IActionResult GetCompensation(String id)
		{
			_logger.LogDebug($"Received compensation get request for employee '{id}'");

			var compensation = _employeeService.GetCompensationByEmployeeId(id);

			if (compensation == null)
				return NotFound();

			return Ok(compensation);
		}
		#endregion
	}
}
