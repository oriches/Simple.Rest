using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Simple.Rest.Tests.Dto;

namespace Simple.Rest.Tests.Controllers
{
    [ApiController]
    [Route("/api/employees")]
    public class EmployeesController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetAllEmployees()
        {
            var result = new ActionResult<IEnumerable<Employee>>(Database.Employees.ToArray());
            return await Task.FromResult(result);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Employee>> Get(int id)
        {
            var (hKey, hValue) = Request.Headers.FirstOrDefault(h => h.Key == "TestHeader");

            var (cKey, cValue) = Request.Cookies.FirstOrDefault(c => c.Key == "TestCookie");

            var employee = Database.Employees.FirstOrDefault(p => p.Id == id);
            if (employee == null) return NotFound();

            if (hKey != null) Response.Headers.Add("TestHeader", hValue);

            if (cKey != null) Response.Cookies.Append("TestCookie", cValue);

            var result = new ActionResult<Employee>(employee);
            return await Task.FromResult(result);
        }

        [HttpPut("{id:int}")]
        public async Task Put(int id, [FromBody] Employee employee)
        {
            var existEmployee = Database.Employees.FirstOrDefault(p => p.Id == id);
            if (existEmployee == null)
            {
                Database.Employees.Add(employee);
            }
            else
            {
                existEmployee.FirstName = employee.FirstName;
                existEmployee.LastName = employee.LastName;
            }

            await Task.CompletedTask;
        }

        [HttpPost]
        public async Task<RedirectResult> Post([FromBody] Employee employee)
        {
            var maxId = Database.Employees.Max(e => e.Id);
            var newId = ++maxId;

            employee.Id = newId;
            Database.Employees.Add(employee);

            var uri = $"{Request.Path}/{employee.Id}";

            return await Task.FromResult(Redirect(uri));
        }

        [HttpDelete("{id:int}")]
        public async Task Delete(int id)
        {
            var existEmployee = Database.Employees.FirstOrDefault(p => p.Id == id);
            if (existEmployee != null) Database.Employees.Remove(existEmployee);

            await Task.CompletedTask;
        }
    }
}