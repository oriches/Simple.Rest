namespace Simple.Rest.Tests.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Web.Http;
    using Dto;

    public class ReportsController : ApiController
    {
        public static IEnumerable<Report> Reports = new List<Report>();

        public static TimeSpan Delay = new TimeSpan(0);

        public Report Get(int id)
        {
            System.Threading.Thread.SpinWait((int) Delay.Ticks);

            var report = Reports.FirstOrDefault((p) => p.Id == id);
            if (report == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return report;
        }
    }
}