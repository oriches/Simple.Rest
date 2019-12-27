using System;
using Simple.Rest.Core.Tests.Infrastructure;

namespace Simple.Rest.Core.TestHarness
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var baseUrl = $"http://{Environment.MachineName}:8081/";
            var service = new TestService(baseUrl);

            Console.WriteLine("Press ENTER close...");
            Console.ReadLine();

            service.Dispose();
        }
    }
}