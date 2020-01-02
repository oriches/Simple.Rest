using System;
using System.Collections.Generic;

namespace Simple.Rest.Tests.Dto
{
    public static class Database
    {
        public static TimeSpan Delay;

        public static IList<Employee> Employees;

        public static IEnumerable<Report> Reports;

        public static void Reset()
        {
            Employees = new List<Employee>
            {
                new Employee {Id = 1, FirstName = "Ollie", LastName = "Riches"},
                new Employee {Id = 2, FirstName = "Steve", LastName = "Austin"}
            };

            Delay = TimeSpan.FromSeconds(3);
            Reports = new List<Report>
            {
                new Report {Id = 1, Name = "SlowReport"}
            };
        }
    }
}