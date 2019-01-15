using System;
using System.Collections.Generic;
using System.Fabric;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Microsoft.ServiceFabric.Data;
using Microsoft.Extensions.Configuration;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System.Text;
using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace MedicalBookingSystem.APIGateway
{
    /// <summary>
    /// The FabricRuntime creates an instance of this class for each service type instance. 
    /// </summary>
    internal sealed class APIGateway : StatelessService
    {
        public APIGateway(StatelessServiceContext context)
            : base(context)
        { }

        /// <summary>
        // Optional override to create listeners(like tcp, http) for this service instance.
        /// </summary>
        // <returns>The collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new ServiceInstanceListener[]
           {
                new ServiceInstanceListener(serviceContext =>
                    new KestrelCommunicationListener(serviceContext, "ServiceEndpoint", (url, listener) =>
                    {
                        ServiceEventSource.Current.ServiceMessage(serviceContext, $"Starting Kestrel on {url}");

                        return new WebHostBuilder()
                                    .UseKestrel()
                                    .ConfigureServices(
                                        services => services
                                             .AddSingleton<HttpClient>(new HttpClient())
                                             .AddSingleton<StatelessServiceContext>(serviceContext))
                                    .ConfigureLogging((hostingContext, logging) =>
                                    {
                                        logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                                        logging.AddConsole();
                                        logging.AddDebug();
                                        logging.AddEventSourceLogger();
                                    })
                                    .UseContentRoot(Directory.GetCurrentDirectory())
                                    .UseServiceFabricIntegration(listener, ServiceFabricIntegrationOptions.None)
                                    .UseUrls(url)
                                    .ConfigureAppConfiguration((hostingContext, config) =>
                                    {
                                        config
                                            .SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
                                            .AddJsonFile("appsettings.json", true, true)
                                            .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", true, true)
                                            .AddJsonFile("ocelot.json", false, false)
                                            .AddEnvironmentVariables();
                                    })
                                    //.ConfigureServices(s => {
                                    //    s.AddOcelot();
                                    //})
                                    //.Configure(a => {
                                    //    a.UseOcelot().Wait();
                                    //   // ConfigureSignalR();
                                    //})
                                    .UseStartup<Startup>()
                                    .Build();
                    }))
           };
        }
    }
}
