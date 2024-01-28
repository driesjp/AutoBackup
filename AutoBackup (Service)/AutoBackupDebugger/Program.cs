using System;
using System.Threading.Tasks;
using AutoBackup; // service namespace
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AutoBackupDebugger
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // create a host builder
            var hostBuilder = new HostBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    // register service class
                    services.AddHostedService<Worker>();
                });

            // build and start the host
            var host = hostBuilder.Build();
            await host.StartAsync();

            Console.WriteLine("Service running... Press any key to stop.");
            Console.ReadKey();

            await host.StopAsync();
        }
    }
}
