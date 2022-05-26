using System.Net;
using System.Net.Http;
using System.Text;
using challenge.Models;
using code_challenge.Tests.Integration.Extensions;
using code_challenge.Tests.Integration.Helpers;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace code_challenge.Tests.Integration
{
	[TestClass]
	public class EmployeeControllerTests
	{
		private static HttpClient _httpClient;
		private static TestServer _testServer;

		[ClassInitialize]
		public static void InitializeClass(TestContext context)
		{
			_testServer = new TestServer(WebHost.CreateDefaultBuilder()
				.UseStartup<TestServerStartup>()
				.UseEnvironment("Development"));

			_httpClient = _testServer.CreateClient();
		}

		[ClassCleanup]
		public static void CleanUpTest()
		{
			_httpClient.Dispose();
			_testServer.Dispose();
		}

		[TestMethod]
		public void CreateEmployee_Returns_Created()
		{
			// Arrange
			var employee = new Employee()
			{
				Department = "Complaints",
				FirstName = "Debbie",
				LastName = "Downer",
				Position = "Receiver",
			};

			var requestContent = new JsonSerialization().ToJson(employee);

			// Execute
			var postRequestTask = _httpClient.PostAsync("api/employee",
			   new StringContent(requestContent, Encoding.UTF8, "application/json"));
			var response = postRequestTask.Result;

			// Assert
			Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

			var newEmployee = response.DeserializeContent<Employee>();
			Assert.IsNotNull(newEmployee.EmployeeId);
			Assert.AreEqual(employee.FirstName, newEmployee.FirstName);
			Assert.AreEqual(employee.LastName, newEmployee.LastName);
			Assert.AreEqual(employee.Department, newEmployee.Department);
			Assert.AreEqual(employee.Position, newEmployee.Position);
		}

		[TestMethod]
		public void GetEmployeeById_Returns_Ok()
		{
			// Arrange
			var employeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f";
			var expectedFirstName = "John";
			var expectedLastName = "Lennon";

			// Execute
			var getRequestTask = _httpClient.GetAsync($"api/employee/{employeeId}");
			var response = getRequestTask.Result;

			// Assert
			Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
			var employee = response.DeserializeContent<Employee>();
			Assert.AreEqual(expectedFirstName, employee.FirstName);
			Assert.AreEqual(expectedLastName, employee.LastName);
		}

		[TestMethod]
		public void UpdateEmployee_Returns_Ok()
		{
			// Arrange
			var employee = new Employee()
			{
				EmployeeId = "03aa1462-ffa9-4978-901b-7c001562cf6f",
				Department = "Engineering",
				FirstName = "Pete",
				LastName = "Best",
				Position = "Developer VI",
			};
			var requestContent = new JsonSerialization().ToJson(employee);

			// Execute
			var putRequestTask = _httpClient.PutAsync($"api/employee/{employee.EmployeeId}",
			   new StringContent(requestContent, Encoding.UTF8, "application/json"));
			var putResponse = putRequestTask.Result;

			// Assert
			Assert.AreEqual(HttpStatusCode.OK, putResponse.StatusCode);
			var newEmployee = putResponse.DeserializeContent<Employee>();

			Assert.AreEqual(employee.FirstName, newEmployee.FirstName);
			Assert.AreEqual(employee.LastName, newEmployee.LastName);
		}

		[TestMethod]
		public void UpdateEmployee_Returns_NotFound()
		{
			// Arrange
			var employee = new Employee()
			{
				EmployeeId = "Invalid_Id",
				Department = "Music",
				FirstName = "Sunny",
				LastName = "Bono",
				Position = "Singer/Song Writer",
			};
			var requestContent = new JsonSerialization().ToJson(employee);

			// Execute
			var postRequestTask = _httpClient.PutAsync($"api/employee/{employee.EmployeeId}",
			   new StringContent(requestContent, Encoding.UTF8, "application/json"));
			var response = postRequestTask.Result;

			// Assert
			Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
		}

		[TestMethod]
		public void GetReportingStructure_Returns_Ok()
		{
			// Arrange
			var employeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f";
			var expectedFirstName = "John";
			var expectedLastName = "Lennon";

			//var employeeId = "03aa1462-ffa9-4978-901b-7c001562cf6f";
			//var expectedFirstName = "Ringo";
			//var expectedLastName = "Lennon";

			// Execute
			var getRequestTask = _httpClient.GetAsync($"api/employee/GetReportingStructure/{employeeId}");
			var response = getRequestTask.Result;

			// Assert
			Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
			var reportingStructure = response.DeserializeContent<ReportingStructure>();
			Assert.AreEqual(expectedFirstName, reportingStructure.employee.FirstName);
			Assert.AreEqual(expectedLastName, reportingStructure.employee.LastName);
			Assert.IsNotNull(reportingStructure.numberOfReports);
		}

		[TestMethod]
		public void CreateCompensation_Returns_Created()
		{
			string EmployeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f";

			// Arrange
			var compensation = new Compensation()
			{
				employee = new Employee()
				{
					EmployeeId = EmployeeId,
					FirstName = "John",
					LastName = "Lennon",
					Position = "Development Manager",
					Department = "Engineering",
				},
				salary = 1500,
				effectiveDate = System.DateTime.Now,
			};

			var requestContent = new JsonSerialization().ToJson(compensation);

			// Execute
			var postRequestTask = _httpClient.PostAsync("api/employee/CreateCompensation",
			   new StringContent(requestContent, Encoding.UTF8, "application/json"));
			var response = postRequestTask.Result;

			// Assert
			Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

			var newCompensation = response.DeserializeContent<Compensation>();
			Assert.IsNotNull(newCompensation.CompensationId);
			Assert.AreEqual(compensation.salary, newCompensation.salary);
			Assert.AreEqual(compensation.effectiveDate, newCompensation.effectiveDate);

			Assert.IsNotNull(newCompensation.employee.EmployeeId);
			Assert.AreEqual(compensation.employee.FirstName, newCompensation.employee.FirstName);
			Assert.AreEqual(compensation.employee.LastName, newCompensation.employee.LastName);
			Assert.AreEqual(compensation.employee.Department, newCompensation.employee.Department);
			Assert.AreEqual(compensation.employee.Position, newCompensation.employee.Position);

			// Arrange

			// Execute
			var getRequestTask = _httpClient.GetAsync($"api/employee/GetCompensation/{EmployeeId}");
			response = getRequestTask.Result;

			// Assert
			Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

			newCompensation = response.DeserializeContent<Compensation>();
			Assert.IsNotNull(newCompensation.CompensationId);
			Assert.AreEqual(compensation.salary, newCompensation.salary);
			Assert.AreEqual(compensation.effectiveDate, newCompensation.effectiveDate);

			Assert.IsNotNull(newCompensation.employee.EmployeeId);
			Assert.AreEqual(compensation.employee.FirstName, newCompensation.employee.FirstName);
			Assert.AreEqual(compensation.employee.LastName, newCompensation.employee.LastName);
			Assert.AreEqual(compensation.employee.Department, newCompensation.employee.Department);
			Assert.AreEqual(compensation.employee.Position, newCompensation.employee.Position);

		}



	}
}
