using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Simple.Rest.Core.Tests.Dto;

namespace Simple.Rest.Core.Tests.Controllers
{
    [ApiController]
    [Route("/api/reports")]
    public class ReportsController : ControllerBase
    {
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Report>> Get(int id)
        {
            Thread.SpinWait((int) Database.Delay.Ticks);

            var report = Database.Reports.FirstOrDefault(p => p.Id == id);
            if (report == null) return NotFound();

            var result = new ActionResult<Report>(report);
            return await Task.FromResult(result);
        }
    }
}