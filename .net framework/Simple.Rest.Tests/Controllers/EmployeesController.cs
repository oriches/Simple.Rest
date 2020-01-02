using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using Simple.Rest.Tests.Dto;

namespace Simple.Rest.Tests.Controllers
{
    public class EmployeesController : ApiController
    {
        public static IList<Employee> Employees = new List<Employee>();

        public IEnumerable<Employee> GetAllEmployees()
        {
            return Employees;
        }

        public HttpResponseMessage Get(int id)
        {
            var testHeader = Request.Headers.FirstOrDefault(h => h.Key == "TestHeader");

            var cookies = Request.Headers.GetCookies();
            var testCookie = cookies.FirstOrDefault(c => c.Cookies.Contains(new CookieState("TestCookie")));

            var employee = Employees.FirstOrDefault(p => p.Id == id);
            if (employee == null) throw new HttpResponseException(HttpStatusCode.NotFound);

            var response = Request.CreateResponse(HttpStatusCode.OK, employee);

            if (testHeader.Key != null) response.Headers.Add("TestHeader", testHeader.Value);

            if (testCookie != null)
                response.Headers.AddCookies(new[]
                    {new CookieHeaderValue("TestCookie", testCookie["TestCookie"].Value)});

            return response;
        }

        public void Put(int id, [FromBody] Employee employee)
        {
            var existEmployee = Employees.FirstOrDefault(p => p.Id == id);
            if (existEmployee == null)
            {
                Employees.Add(employee);
            }
            else
            {
                existEmployee.FirstName = employee.FirstName;
                existEmployee.LastName = employee.LastName;
            }
        }

        public HttpResponseMessage Post([FromBody] Employee employee)
        {
            var maxId = Employees.Max(e => e.Id);
            var newId = ++maxId;

            employee.Id = newId;
            Employees.Add(employee);

            var uri = Url.Link("DefaultApi", new {id = employee.Id});

            var response = Request.CreateResponse(HttpStatusCode.Redirect);
            response.Headers.Location = new Uri(uri);
            return response;
        }

        public HttpResponseMessage Delete(int id)
        {
            var existEmployee = Employees.FirstOrDefault(p => p.Id == id);
            if (existEmployee != null) Employees.Remove(existEmployee);

            return new HttpResponseMessage(HttpStatusCode.NoContent);
        }
    }
}