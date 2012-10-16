﻿namespace Simple.Rest.Tests
{
    using System;
    using Dto;
    using NUnit.Framework;
    using Rest;
    using Serializers;

    [TestFixture]
    public class RestClientPostTests
    {
        private IRestClient _jsonRestClient;
        private IRestClient _xmlRestClient;

        private string _baseUrl;
        private TestService _testService;
        
        [Test]
        public void should_post_json_object()
        {
            // ARRANGE
            var url = new Uri(_baseUrl + "/api/employees");
            
            // ACT
            var newEmployee = new Employee {Id = 3, FirstName = "Alex", LastName = "Chauhan"};
            var task = _jsonRestClient.PostAsync(url, newEmployee);
            task.Wait();
            
            var result = task.Result.Resource;
             
            // ASSIGN
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(newEmployee.Id));
            Assert.That(result.FirstName, Is.EqualTo(newEmployee.FirstName));
            Assert.That(result.LastName, Is.EqualTo(newEmployee.LastName));
        }

        [Test]
        public void should_post_xml_object()
        {
            // ARRANGE
            var url = new Uri(_baseUrl + "/api/employees");

            // ACT
            var newEmployee = new Employee { Id = 3, FirstName = "Alex", LastName = "Chauhan" };
            var task = _xmlRestClient.PostAsync(url, newEmployee);
            task.Wait();

            var result = task.Result.Resource;

            // ASSIGN
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(newEmployee.Id));
            Assert.That(result.FirstName, Is.EqualTo(newEmployee.FirstName));
            Assert.That(result.LastName, Is.EqualTo(newEmployee.LastName));
        }

        [TestFixtureSetUp]
        public void SetUp()
        {
            _baseUrl = string.Format("http://{0}:8082", Environment.MachineName);

            _testService = new TestService(_baseUrl);

            _jsonRestClient = new RestClient(new JsonSerializer());
            _xmlRestClient = new RestClient(new XmlSerializer());
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            _testService.Dispose();
        }
    }
}