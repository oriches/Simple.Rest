using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using Microsoft.Reactive.Testing;
using NUnit.Framework;
using Simple.Rest.Common;
using Simple.Rest.Common.Extensions;
using Simple.Rest.Common.Serializers;
using Simple.Rest.Tests.Dto;
using Simple.Rest.Tests.Infrastructure;

namespace Simple.Rest.Tests
{
    [TestFixture]
    public class RestClientGetTests
    {
        [SetUp]
        public void SetUp()
        {
            _jsonRestClient.Headers.Clear();
            _xmlRestClient.Headers.Clear();

            Database.Reset();
        }

        private IRestClient _jsonRestClient;
        private IRestClient _xmlRestClient;

        private string _baseUrl;
        private string _invalidHostUrl;
        private string _invalidPortUrl;

        private TestService _testService;
        private TestScheduler _testScheduler;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _baseUrl = $"http://{Environment.MachineName}:8080";
            _invalidPortUrl = $"http://{Environment.MachineName}:8079";
            _invalidHostUrl = $"http://{Environment.MachineName}1:8079";

            _testScheduler = new TestScheduler();

            _testService = new TestService(_baseUrl);

            _jsonRestClient = new RestClient(new JsonSerializer());
            _xmlRestClient = new RestClient(new XmlSerializer());
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _testService.Dispose();
        }

        [Test]
        public void should_fail_when_resource_id_is_invalid()
        {
            // ARRANGE
            var url = new Uri(_invalidHostUrl + "/api/employees/99");
            var sync = new ManualResetEvent(false);

            // ACT
            Exception exn = null;
            _jsonRestClient.GetAsync<Employee>(url)
                .ToObservable()
                .Take(1)
                .Subscribe(_ => { }, e =>
                {
                    exn = e;
                    sync.Set();
                });

            // ASSERT
            sync.WaitOne();
            Assert.That(exn, Is.Not.Null);
        }

        [Test]
        public void should_fail_when_url_is_invalid_controller()
        {
            // ARRANGE
            var url = new Uri(_baseUrl + "/api/documents/1");
            var sync = new ManualResetEvent(false);

            // ACT
            Exception exn = null;
            _jsonRestClient.GetAsync<Employee>(url)
                .ToObservable()
                .Take(1)
                .Subscribe(_ => { }, e =>
                {
                    exn = e;
                    sync.Set();
                });

            // ASSERT
            sync.WaitOne();
            Assert.That(exn, Is.Not.Null);
        }

        [Test]
        public void should_fail_when_url_is_invalid_host()
        {
            // ARRANGE
            var url = new Uri(_invalidHostUrl + "/api/employees/1");
            var sync = new ManualResetEvent(false);

            // ACT
            Exception exn = null;
            _jsonRestClient.GetAsync<Employee>(url)
                .ToObservable()
                .Take(1)
                .Subscribe(_ => { }, e =>
                {
                    exn = e;
                    sync.Set();
                });

            // ASSERT
            sync.WaitOne();
            Assert.That(exn, Is.Not.Null);
        }

        [Test]
        public void should_fail_when_url_is_invalid_port()
        {
            // ARRANGE
            var url = new Uri(_invalidPortUrl + "/api/documents/1");
            var sync = new ManualResetEvent(false);

            // ACT
            Exception exn = null;
            _jsonRestClient.GetAsync<Employee>(url)
                .ToObservable()
                .Take(1)
                .Subscribe(_ => { }, e =>
                {
                    exn = e;
                    sync.Set();
                });

            // ASSERT
            sync.WaitOne();
            Assert.That(exn, Is.Not.Null);
        }

        [Test]
        public void should_fail_with_timeout()
        {
            // ARRANGE
            var timeout = Database.Delay.Add(new TimeSpan(0, 0, 0, 1));
            var url = new Uri(_baseUrl + "/api/reports/1");

            // ACT
            Exception exn = null;
            _jsonRestClient.GetAsync<Report>(url)
                .ToObservable()
                .Timeout(timeout, _testScheduler)
                .Take(1)
                .Subscribe(_ => { }, e => exn = e);

            _testScheduler.AdvanceBy(timeout.Ticks);

            // ASSERT
            Assert.That(exn, Is.Not.Null);
        }

        [Test]
        public void should_return_expected_cookie()
        {
            // ARRANGE
            var url = new Uri(_baseUrl + "/api/employees/1");

            var requestCookie = new Cookie("TestCookie", Guid.NewGuid().ToString());
            _jsonRestClient.WithCookie(requestCookie);

            // ACT
            var task = _jsonRestClient.GetAsync<Employee>(url);
            task.Wait();

            var responseCookie = _jsonRestClient.Cookies["TestCookie"];

            // ASSERT
            Assert.That(requestCookie, Is.EqualTo(responseCookie));
        }

        [Test]
        public void should_return_expected_header()
        {
            // ARRANGE
            var url = new Uri(_baseUrl + "/api/employees/1");

            var requestHeaderValue = Guid.NewGuid().ToString();
            _jsonRestClient.Headers.Add("TestHeader", requestHeaderValue);

            // ACT
            var task = _jsonRestClient.GetAsync<Employee>(url);
            task.Wait();

            var responseHeaderValue = _jsonRestClient.Headers["TestHeader"];

            // ASSERT
            Assert.That(requestHeaderValue, Is.EqualTo(responseHeaderValue));
        }

        [Test]
        public void should_return_list_of_json_objects()
        {
            // ARRANGE
            var url = new Uri(_baseUrl + "/api/employees");

            // ACT
            var task = _jsonRestClient.GetAsync<IEnumerable<Employee>>(url);
            task.Wait();

            var employees = task.Result.Resource;

            // ASSIGN
            CollectionAssert.AreEquivalent(Database.Employees, employees);
        }

        [Test]
        public void should_return_list_of_xml_objects()
        {
            // ARRANGE
            var url = new Uri(_baseUrl + "/api/employees");

            // ACT
            var task = _xmlRestClient.GetAsync<IEnumerable<Employee>>(url);
            task.Wait();

            var employees = task.Result.Resource;

            // ASSIGN
            CollectionAssert.AreEquivalent(Database.Employees, employees);
        }

        [Test]
        public void should_return_single_json_object()
        {
            // ARRANGE
            var employees = Database.Employees;
            var url = new Uri(_baseUrl + "/api/employees/1");

            // ACT
            var task = _jsonRestClient.GetAsync<Employee>(url);
            task.Wait();

            var employee = task.Result.Resource;

            // ASSERT
            Assert.That(employee, Is.Not.Null);
            Assert.That(employee.Id, Is.EqualTo(employees.First().Id));
            Assert.That(employee.FirstName, Is.EqualTo(employees.First().FirstName));
            Assert.That(employee.LastName, Is.EqualTo(employees.First().LastName));
        }

        [Test]
        public void should_return_single_json_object_with_brotli_compressions()
        {
            // ARRANGE
            var employees = Database.Employees;
            var url = new Uri(_baseUrl + "/api/employees/1");

            // ACT
            var task = _jsonRestClient.WithBrotliEncoding()
                .GetAsync<Employee>(url);

            task.Wait();

            var employee = task.Result.Resource;

            // ASSERT
            Assert.That(employee, Is.Not.Null);
            Assert.That(employee.Id, Is.EqualTo(employees.First().Id));
            Assert.That(employee.FirstName, Is.EqualTo(employees.First().FirstName));
            Assert.That(employee.LastName, Is.EqualTo(employees.First().LastName));
        }

        [Test]
        public void should_return_single_json_object_with_deflate_compressions()
        {
            // ARRANGE
            var employees = Database.Employees;
            var url = new Uri(_baseUrl + "/api/employees/1");

            // ACT
            var task = _jsonRestClient.WithDeflateEncoding()
                .GetAsync<Employee>(url);

            task.Wait();

            var employee = task.Result.Resource;

            // ASSERT
            Assert.That(employee, Is.Not.Null);
            Assert.That(employee.Id, Is.EqualTo(employees.First().Id));
            Assert.That(employee.FirstName, Is.EqualTo(employees.First().FirstName));
            Assert.That(employee.LastName, Is.EqualTo(employees.First().LastName));
        }

        [Test]
        public void should_return_single_json_object_with_gzip_compressions()
        {
            // ARRANGE
            var employees = Database.Employees;
            var url = new Uri(_baseUrl + "/api/employees/1");

            // ACT
            var task = _jsonRestClient.WithGzipEncoding()
                .GetAsync<Employee>(url);

            task.Wait();

            var employee = task.Result.Resource;

            // ASSERT
            Assert.That(employee, Is.Not.Null);
            Assert.That(employee.Id, Is.EqualTo(employees.First().Id));
            Assert.That(employee.FirstName, Is.EqualTo(employees.First().FirstName));
            Assert.That(employee.LastName, Is.EqualTo(employees.First().LastName));
        }

        [Test]
        public void should_return_single_xml_object()
        {
            // ARRANGE
            var employees = Database.Employees;
            var url = new Uri(_baseUrl + "/api/employees/1");

            // ACT
            var task = _xmlRestClient.GetAsync<Employee>(url);
            task.Wait();

            var employee = task.Result.Resource;

            // ASSERT
            Assert.That(employee, Is.Not.Null);
            Assert.That(employee.Id, Is.EqualTo(employees.First().Id));
            Assert.That(employee.FirstName, Is.EqualTo(employees.First().FirstName));
            Assert.That(employee.LastName, Is.EqualTo(employees.First().LastName));
        }
    }
}