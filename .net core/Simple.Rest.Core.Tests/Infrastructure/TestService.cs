using System;
using System.Reactive.Disposables;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Simple.Rest.Tests.Infrastructure
{
    public sealed class TestService : IDisposable
    {
        private readonly CompositeDisposable _disposable = new CompositeDisposable();

        public TestService(string baseUrl)
        {
            var host = WebHost.CreateDefaultBuilder()
                .UseKestrel()
                .UseUrls(baseUrl)
                .UseStartup<Startup>()
                .Build();

            _disposable.Add(host);

            host.StartAsync();
            _disposable.Add(Disposable.Create(() =>
            {
                try
                {
                    host.Dispose();
                }
                catch (Exception)
                {
                    //swallow...
                }
            }));
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }
    }
}