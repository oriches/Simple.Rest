# Simple.Rest

[![NuGet](https://img.shields.io/nuget/v/simple.rest.svg)](https://github.com/oriches/simple.rest)
[![Build status](https://ci.appveyor.com/api/projects/status/pnqpfl7eeytfmxn2/branch/master?svg=true)](https://ci.appveyor.com/project/oriches/simple-rest)

As with all my 'important' stuff it builds using the amazing [AppVeyor](https://ci.appveyor.com/project/oriches/simple-rest).

Supported versions:

    .Net Framework v4.8 and higher,
    .Net Core v3.1 and higher,
    
A .Net class library to simplify the calling of a RESTful web service from a .Net client application.

For more information about the releases see [Release Info] (https://github.com/oriches/Simple.Rest/wiki/Release-Info).

This library is available as a nuget [package] (https://nuget.org/packages/Simple.Rest).

You can skip the intro and go straight to the [Getting Started] (https://github.com/oriches/Simple.Rest/wiki/Getting-Started) guide.

This class library makes use of the 'async & await' keywords available in the standard install of .Net 4.5 and by the inclusion of the <a href="https://www.nuget.org/packages/Microsoft.Bcl.Async">Microsoft.Bcl.Async</a> NuGet package for .Net 4.0, this means .Net 4.0 applications wanting to use this will have to have the Microsoft Hotfix <a href="http://support.microsoft.com/kb/2468871">KB2468871</a> installed.

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
            
    var task = await restClient.GetAsync<Employee>(url);
    
    var employee = task.Result.Resource;
```
Similarly to call a RESTful XML web service would be:
```C#
    var url = new Uri("http://localhost:8080/api/employees/1");
    var restClient = new RestClient(new XmlSerializer());
            
    var task = await restClient.GetAsync<Employee>(url);
    
    var employee = task.Result.Resource;
```
As you can see from the examples above the library makes use of the Task<T> metaphor from the .Net framework to execute the asynchronous request over the wire.

The library was developed using TDD principles and has a set of test in the solution for all the main HTTP operations GET, POST, PUT & DELETE. For more information see the [Getting Started] (https://github.com/oriches/Simple.Rest/wiki/Getting-Started) guide.
