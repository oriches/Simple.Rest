using System;
using System.Net;
using NUnit.Framework;
using Simple.Rest.Common;
using Simple.Rest.Common.Serializers;
using Simple.Rest.Tests.Dto;
using Simple.Rest.Tests.Infrastructure;

namespace Simple.Rest.Tests
{
    [TestFixture]
    public class RestClientDeleteTests
    {
        [SetUp]
        public void SetUp()
        {
            _jsonRestClient.Headers.Clear();
            _xmlRestClient.Headers.Clear();

            Database.Reset();
        }

        private IRestClient _xmlRestClient;
        private IRestClient _jsonRestClient;

        private string _baseUrl;
        private TestService _testService;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _baseUrl = $"http://{Environment.MachineName}:8083";

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
        public void should_delete_json_object()
        {
            // ARRANGE
            var url = new Uri(_baseUrl + "/api/employees/1");

            // ACT
            var task = _jsonRestClient.DeleteAsync(url);
            task.Wait();

            var response = task.Result;

            // ASSIGN
            Assert.That(response, Is.Not.Null);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public void should_delete_xml_object()
        {
            // ARRANGE
            var url = new Uri(_baseUrl + "/api/employees/1");

            // ACT
            var task = _xmlRestClient.DeleteAsync(url);
            task.Wait();

            var response = task.Result;

            // ASSIGN
            Assert.That(response, Is.Not.Null);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }
    }
}