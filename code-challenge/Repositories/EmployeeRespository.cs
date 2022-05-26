using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using challenge.Data;
using challenge.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace challenge.Repositories
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
			return _employeeContext.Employees.SingleOrDefault(e => e.EmployeeId == id);
		}

		public Task SaveAsync()
		{
			return _employeeContext.SaveChangesAsync();
		}

		public Employee Remove(Employee employee)
		{
			return _employeeContext.Remove(employee).Entity;
		}

		#region KMONAHAN: ReportingStructure
		public ReportingStructure GetReportingStructure(string id)
		{
			var emp = (from e in _employeeContext.Employees.Include(dr => dr.DirectReports)
					   where e.EmployeeId == id
					   select e).FirstOrDefault();

			int numReports = GetDirectReports(emp).Count;

			return new ReportingStructure { employee = emp, numberOfReports = numReports };
		}

		private List<Employee> GetDirectReports(Employee parentEmployee)
		{
			var returnList = new List<Employee>();

			if (parentEmployee?.DirectReports == null) //not concerned with reords with zero DirectReports
			{
				return new List<Employee>();
			}

			//Create list of string for IN query
			HashSet<string> DirectReportIdList =
					(from dr in parentEmployee.DirectReports
					 where dr != null
					 select dr.EmployeeId).ToHashSet<string>();

			var employees = _employeeContext.Employees.Include(dr => dr.DirectReports)
									   .Where(x => DirectReportIdList.Contains(x.EmployeeId))
									   .ToList();

			foreach (var employee in employees)
			{
				returnList.Add(employee);
				returnList.AddRange(GetDirectReports(employee));
			}
			return returnList;
		}
		#endregion

		#region KMONAHAN: Compensation
		public Compensation AddCompensation(Compensation compensation)
		{
			compensation.CompensationId = Guid.NewGuid().ToString();
			_employeeContext.Compensations.Add(compensation);
			_employeeContext.Entry(compensation.employee).State = EntityState.Unchanged; //don't save employee entity.

			return compensation;
		}

		public Compensation GetCompensationByEmployeeId(string id)
		{
			var emp = (from e in _employeeContext.Compensations.Include(dr => dr.employee)
					   where e.employee.EmployeeId == id
					   select e).FirstOrDefault();

			return emp;
		}
		#endregion

	}
}
