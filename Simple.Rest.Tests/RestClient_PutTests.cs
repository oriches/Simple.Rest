﻿namespace Simple.Rest.Tests
{
    using System;
    using Dto;
    using Extensions;
    using Infrastructure;
    using NUnit.Framework;
    using Rest;
    using Serializers;

    [TestFixture]
    public class RestClientPutTests
    {
        private IRestClient _jsonRestClient;
        private IRestClient _xmlRestClient;

        private string _baseUrl;
        private TestService _testService;

        [Test]
        public void should_put_json_object()
        {
            // ARRANGE
            var url = new Uri(_baseUrl + "/api/employees/1");
            var employee = GetEmployee(url);

            // ACT
            employee.FirstName = "Oliver";
           
            var task = _jsonRestClient.PutAsync(url, employee);
            task.Wait();

            var response = task.Result;
            
            // ASSIGN
            var updatedEmployee = GetEmployee(url);
            
            Assert.That(response, Is.Not.Null);
            Assert.That(updatedEmployee.Id, Is.EqualTo(employee.Id));
            Assert.That(updatedEmployee.FirstName, Is.EqualTo(employee.FirstName));
            Assert.That(updatedEmployee.LastName, Is.EqualTo(employee.LastName));
        }

        [Test, Ignore("GZip not supported")]
        public void should_put_json_object_with_gzip_compression()
        {
            // ARRANGE
            var url = new Uri(_baseUrl + "/api/employees/1");
            var employee = GetEmployee(url);

            // ACT
            employee.FirstName = "Ollie";

            var task = _jsonRestClient.WithGzipEncoding()
                .PutAsync(url, employee);

            task.Wait();

            var response = task.Result;

            // ASSIGN
            var updatedEmployee = GetEmployee(url);

            Assert.That(response, Is.Not.Null);
            Assert.That(updatedEmployee.Id, Is.EqualTo(employee.Id));
            Assert.That(updatedEmployee.FirstName, Is.EqualTo(employee.FirstName));
            Assert.That(updatedEmployee.LastName, Is.EqualTo(employee.LastName));
        }

        [Test, Ignore("Deflate not supported")]
        public void should_put_json_object_with_deflate_compression()
        {
            // ARRANGE
            var url = new Uri(_baseUrl + "/api/employees/1");
            var employee = GetEmployee(url);

            // ACT
            employee.FirstName = "Oliver";

            var task = _jsonRestClient.WithDeflateEncoding()
                .PutAsync(url, employee);

            task.Wait();

            var response = task.Result;

            // ASSIGN
            var updatedEmployee = GetEmployee(url);

            Assert.That(response, Is.Not.Null);
            Assert.That(updatedEmployee.Id, Is.EqualTo(employee.Id));
            Assert.That(updatedEmployee.FirstName, Is.EqualTo(employee.FirstName));
            Assert.That(updatedEmployee.LastName, Is.EqualTo(employee.LastName));
        }

        [Test]
        public void should_put_xml_object()
        {
            // ARRANGE
            var url = new Uri(_baseUrl + "/api/employees/2");
            var employee = GetEmployee(url);

            // ACT
            employee.FirstName = "Oliver";

            var task = _xmlRestClient.PutAsync(url, employee);
            task.Wait();

            var response = task.Result;

            // ASSIGN
            var updatedEmployee = GetEmployee(url);

            Assert.That(response, Is.Not.Null);
            Assert.That(updatedEmployee.Id, Is.EqualTo(employee.Id));
            Assert.That(updatedEmployee.FirstName, Is.EqualTo(employee.FirstName));
            Assert.That(updatedEmployee.LastName, Is.EqualTo(employee.LastName));
        }

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            _baseUrl = string.Format("http://{0}:8081", Environment.MachineName);

            _testService = new TestService(_baseUrl);

            _jsonRestClient = new RestClient(new JsonSerializer());
            _xmlRestClient = new RestClient(new XmlSerializer());
        }

        [SetUp]
        public void SetUp()
        {
            _jsonRestClient.Headers.Clear();
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            _testService.Dispose();
        }

        private Employee GetEmployee(Uri url)
        {
            var task = _jsonRestClient.GetAsync<Employee>(url);
            task.Wait();

            return task.Result.Resource;
        }
    }
}
