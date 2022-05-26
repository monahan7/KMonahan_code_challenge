using System;
using challenge.Models;

namespace challenge.Services
{
	public interface IEmployeeService
	{
		Employee GetById(String id);
		Employee Create(Employee employee);
		Employee Replace(Employee originalEmployee, Employee newEmployee);
		ReportingStructure GetReportingStructure(String id);
		Compensation CreateCompensation(Compensation compensation);
		Compensation GetCompensationByEmployeeId(string id);


	}
}
