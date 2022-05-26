using System;
using System.Threading.Tasks;
using challenge.Models;

namespace challenge.Repositories
{
	public interface IEmployeeRepository
	{
		Employee GetById(String id);
		Employee Add(Employee employee);
		Employee Remove(Employee employee);
		Task SaveAsync();
		ReportingStructure GetReportingStructure(String id);
		Compensation AddCompensation(Compensation compensation);
		Compensation GetCompensationByEmployeeId(string id);
	}
}