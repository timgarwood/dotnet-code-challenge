
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CodeChallenge.Models;

using CodeCodeChallenge.Tests.Integration.Extensions;
using CodeCodeChallenge.Tests.Integration.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeCodeChallenge.Tests.Integration
{
    [TestClass]
    public class EmployeeControllerTests
    {
        private static HttpClient _httpClient;
        private static TestServer _testServer;

        [ClassInitialize]
        // Attribute ClassInitialize requires this signature
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        public static void InitializeClass(TestContext context)
        {
            _testServer = new TestServer();
            _httpClient = _testServer.NewClient();
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
        public async Task GetReportingStructure_JohnLennon_Returns_4()
        {
            var response = await _httpClient.GetAsync($"api/employee/structure/16a596ae-edd3-4847-99fe-c4518e82c86f");
            var lennon = response.DeserializeContent<ReportingStructureModel>();

            Assert.AreEqual(lennon.NumberOfReports, 4);
            Assert.AreEqual(lennon.Employee.EmployeeId, "16a596ae-edd3-4847-99fe-c4518e82c86f");
            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK);

            Assert.AreEqual(lennon.Employee.DirectReports.Count, 2);
            Assert.AreEqual(lennon.Employee.DirectReports[0].EmployeeId, "b7839309-3348-463b-a7e3-5de1c168beb3");
            Assert.AreEqual(lennon.Employee.DirectReports[1].EmployeeId, "03aa1462-ffa9-4978-901b-7c001562cf6f");
            Assert.AreEqual(lennon.Employee.DirectReports[1].DirectReports.Count, 2);
            Assert.AreEqual(lennon.Employee.DirectReports[1].DirectReports[0].EmployeeId, "62c1084e-6e34-4630-93fd-9153afb65309");
            Assert.AreEqual(lennon.Employee.DirectReports[1].DirectReports[1].EmployeeId, "c0c2293d-16bd-4603-8e08-638a9d18b22c");

        }

        [TestMethod]
        public async Task GetReportingStructure_RingoStarr_Returns_2()
        {
            var response = await _httpClient.GetAsync($"api/employee/structure/03aa1462-ffa9-4978-901b-7c001562cf6f");
            var ringo = response.DeserializeContent<ReportingStructureModel>();

            Assert.AreEqual(ringo.NumberOfReports, 2);
            Assert.AreEqual(ringo.Employee.EmployeeId, "03aa1462-ffa9-4978-901b-7c001562cf6f");
            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK);
        }

        [TestMethod]
        public async Task GetReportingStructure_GeorgeHarrison_Returns_0()
        {
            var response = await _httpClient.GetAsync($"api/employee/structure/c0c2293d-16bd-4603-8e08-638a9d18b22c");
            var george = response.DeserializeContent<ReportingStructureModel>();

            Assert.AreEqual(george.NumberOfReports, 0);
            Assert.AreEqual(george.Employee.EmployeeId, "c0c2293d-16bd-4603-8e08-638a9d18b22c");
            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK);
        }

        [TestMethod]
        public async Task GetReportingStructure_InvalidId_Returns_NotFound()
        {
            var response = await _httpClient.GetAsync($"api/employee/structure/JasonNewstead");

            Assert.AreEqual(response.StatusCode, HttpStatusCode.NotFound);
        }

        [TestMethod]
        public async Task CreateCompensation_Returns_Created_GetOk()
        {
            // Arrange
            var model = new CompensationCreateModel
            {
                EmployeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f",
                EffectiveDate = new DateOnly(2022, 1, 15),
                Salary = 120000
            };

            var model2 = new CompensationCreateModel
            {
                EmployeeId = "62c1084e-6e34-4630-93fd-9153afb65309",
                EffectiveDate = new DateOnly(2000, 3, 22),
                Salary = 55000
            };


            var model3 = new CompensationCreateModel
            {
                EmployeeId = "JasonNewstead",
                EffectiveDate = new DateOnly(2000, 3, 22),
                Salary = 55000
            };

            var requestContent = new JsonSerialization().ToJson(model);
            var requestContent2 = new JsonSerialization().ToJson(model2);
            var requestContent3 = new JsonSerialization().ToJson(model3);


            var response = await _httpClient.PostAsync($"api/employee/compensation",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));

            var response2 = await _httpClient.PostAsync($"api/employee/compensation",
               new StringContent(requestContent2, Encoding.UTF8, "application/json"));

            var response3 = await _httpClient.PostAsync($"api/employee/compensation",
               new StringContent(requestContent3, Encoding.UTF8, "application/json"));

            Assert.AreEqual(response3.StatusCode, HttpStatusCode.NotFound);

            var compensationModel = response.DeserializeContent<CompensationCreateModel>();
            var compensationModel2 = response2.DeserializeContent<CompensationCreateModel>();


            Assert.AreEqual(compensationModel.EmployeeId, "16a596ae-edd3-4847-99fe-c4518e82c86f");
            Assert.AreEqual(compensationModel.Salary, 120000);
            Assert.AreEqual(compensationModel.EffectiveDate.Year, 2022);
            Assert.AreEqual(compensationModel.EffectiveDate.Month, 1);
            Assert.AreEqual(compensationModel.EffectiveDate.Day, 15);
            Assert.AreEqual(compensationModel2.EmployeeId, "62c1084e-6e34-4630-93fd-9153afb65309");
            Assert.AreEqual(compensationModel2.Salary, 55000);
            Assert.AreEqual(compensationModel2.EffectiveDate.Year, 2000);
            Assert.AreEqual(compensationModel2.EffectiveDate.Month, 3);
            Assert.AreEqual(compensationModel2.EffectiveDate.Day, 22);

            response = await _httpClient.GetAsync($"api/employee/compensation/16a596ae-edd3-4847-99fe-c4518e82c86f");
            response2 = await _httpClient.GetAsync($"api/employee/compensation/62c1084e-6e34-4630-93fd-9153afb65309");


            var compensation = response.DeserializeContent<Compensation>();
            var compensation2 = response2.DeserializeContent<Compensation>();

            Assert.AreEqual(compensation.EmployeeId, "16a596ae-edd3-4847-99fe-c4518e82c86f");
            Assert.AreEqual(compensation.Salary, 120000);
            Assert.AreEqual(compensation.EffectiveDate.Year, 2022);
            Assert.AreEqual(compensation.EffectiveDate.Month, 1);
            Assert.AreEqual(compensation.EffectiveDate.Day, 15);
            Assert.AreEqual(compensation2.EmployeeId, "62c1084e-6e34-4630-93fd-9153afb65309");
            Assert.AreEqual(compensation2.Salary, 55000);
            Assert.AreEqual(compensation2.EffectiveDate.Year, 2000);
            Assert.AreEqual(compensation2.EffectiveDate.Month, 3);
            Assert.AreEqual(compensation2.EffectiveDate.Day, 22);
        }

        [TestMethod]
        public async Task GetCompensation_NoCompensation_Returns_NotFound()
        {
            //valid employee, no compensation
            var response = await _httpClient.GetAsync($"api/employee/compensation/c0c2293d-16bd-4603-8e08-638a9d18b22c");

            Assert.AreEqual(response.StatusCode, HttpStatusCode.NotFound);
        }

        [TestMethod]
        public async Task GetCompensation_NoEmployee_Returns_NotFound()
        {
            //valid employee, no compensation
            var response = await _httpClient.GetAsync($"api/employee/compensation/JasonNewstead");

            Assert.AreEqual(response.StatusCode, HttpStatusCode.NotFound);
        }

        [TestMethod]
        public async Task CreateCompensation_2Compensations_Returns_Conflict()
        {
            // create an employee and give them 2 compensations
            // verify that the 2nd compensation request returns Conflict
            // Arrange
            var employee = new Employee()
            {
                Department = "Metallica",
                FirstName = "James",
                LastName = "Hetfield",
                Position = "Guitar",
            };

            var requestContent = new JsonSerialization().ToJson(employee);

            // Execute
            var response = await _httpClient.PostAsync("api/employee",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));

            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            var james = response.DeserializeContent<Employee>();

            var compensation1 = new Compensation
            {
                EmployeeId = james.EmployeeId,
                EffectiveDate = new DateOnly(1985, 1, 10),
                Salary = 11000
            };

            var compensation2 = new Compensation
            {
                EmployeeId = james.EmployeeId,
                EffectiveDate = new DateOnly(2022, 1, 10),
                Salary = 50000
            };

            var compRequest1 = new JsonSerialization().ToJson(compensation1);
            var compRequest2 = new JsonSerialization().ToJson(compensation2);

            // Execute
            var compResponse1 = await _httpClient.PostAsync("api/employee/compensation",
               new StringContent(compRequest1, Encoding.UTF8, "application/json"));
            var compResponse2 = await _httpClient.PostAsync("api/employee/compensation",
               new StringContent(compRequest2, Encoding.UTF8, "application/json"));

            Assert.AreEqual(compResponse1.StatusCode, HttpStatusCode.OK);
            Assert.AreEqual(compResponse2.StatusCode, HttpStatusCode.Conflict);

            compResponse1 = await _httpClient.GetAsync($"api/employee/compensation/{james.EmployeeId}");

            var jamesComp = compResponse1.DeserializeContent<Compensation>();

            Assert.AreEqual(jamesComp.Salary, 11000);
            Assert.AreEqual(jamesComp.EffectiveDate.Month, 1);
            Assert.AreEqual(jamesComp.EffectiveDate.Day, 10);
            Assert.AreEqual(jamesComp.EffectiveDate.Year, 1985);

        }

    }
}