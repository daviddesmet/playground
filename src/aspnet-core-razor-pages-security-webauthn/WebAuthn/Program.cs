using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace WebAuthn
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    // Custom certificate feed from settings...
                    // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/servers/kestrel?view=aspnetcore-3.1#configurehttpsdefaultsactionhttpsconnectionadapteroptions
                    webBuilder.UseStartup<Startup>();
                });
    }
}
