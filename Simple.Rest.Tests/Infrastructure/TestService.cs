namespace Simple.Rest.Tests.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Web.Http;
    using System.Web.Http.SelfHost;
    using Controllers;
    using Dto;

    public sealed class TestService : IDisposable
    {
        private readonly string _baseUrl;

        private HttpSelfHostServer _server;

        public IList<Employee> Employees;
        public IList<Report> Reports;
        public TimeSpan Delay;

        public TestService(string baseUrl)
        {
            _baseUrl = baseUrl;

            SetUpControllers();
            SetUpHost();
        }

        public void Dispose()
        {
            _server.CloseAsync().Wait();
            _server.Dispose();
        }

        private void SetUpHost()
        {
            var config = new HttpSelfHostConfiguration(_baseUrl);

            config.MessageHandlers.Add(new CompressionHandler());
            config.Routes.MapHttpRoute("DefaultAPI", "api/{controller}/{id}", new {id = RouteParameter.Optional});

            _server = new HttpSelfHostServer(config);
            _server.OpenAsync().Wait();
        }

        private void SetUpControllers()
        {
            Employees = new List<Employee>
                        {
                            new Employee {Id = 1, FirstName = "Ollie", LastName = "Riches"},
                            new Employee {Id = 2, FirstName = "Steve", LastName = "Austin"},
                        };

            EmployeesController.Employees = Employees;

            Delay = TimeSpan.FromSeconds(3);
            Reports = new List<Report>
                      {
                          new Report {Id = 1, Name = "SlowReport"}
                      };

            ReportsController.Delay = Delay;
            ReportsController.Reports = Reports;
        }
    }
}