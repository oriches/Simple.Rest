Simple.Rest
===========

A library to simplify the calling of a RESTful web service from a .Net client application.

# Introduction

This small library is a wrapper around the use of HttpWebRequest\WebRequest classes in the .Net framework and is designed to make the use of RESTful web service in a .Net application as easy as possible. An example will probably help to explain, imaging you're trying to GET an Employee resource from a RESTful web service and lets say the web service supports both JSON & XML resource representations:

The Employee resource looks like this .Net (C#) code:
```C#
public class Employee
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}
```
Using Simple.Rest the code to call the RESTful JSON web service would be:
```C#
    var url = new Uri("http://localhost:8080/api/employees/1");
    var restClient = new RestClient(new JsonSerializer());
            
    var task = restClient.GetAsync<Employee>(url);
    task.Wait();
        
    var employee = task.Result.Resource;
```
Similarly to call a RESTful XML web service would be:
```C#
    var url = new Uri("http://localhost:8080/api/employees/1");
    var restClient = new RestClient(new XmlSerializer());
            
    var task = restClient.GetAsync<Employee>(url);
    task.Wait();
        
    var employee = task.Result.Resource;
```



