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

        //protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        //{
        //    return new[]
        //    {
        //        new ServiceInstanceListener(
        //            initparams => new WebCommunicationListener(string.Empty, initparams),
        //            "OcelotServiceWebListener")
        //    };

        //    //return new ServiceInstanceListener[]
        //    //{
        //    //    new ServiceInstanceListener(serviceContext => new OwinCommunicationListener(Startup.ConfigureApp, serviceContext, ServiceEventSource.Current, "ServiceEndpoint"))
        //    //};
        //}

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
                                            .AddSingleton<StatelessServiceContext>(serviceContext))
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
            //return new ServiceInstanceListener[]
            //{
            //    new ServiceInstanceListener(serviceContext =>
            //        new KestrelCommunicationListener(serviceContext, "ServiceEndpoint", (url, listener) =>
            //        {
            //            ServiceEventSource.Current.ServiceMessage(serviceContext, $"Starting Kestrel on {url}");

            //            return new WebHostBuilder()
            //                        .UseKestrel()
            //                        .ConfigureServices(
            //                            services => services
            //                                .AddSingleton<StatelessServiceContext>(serviceContext))
            //                        .UseContentRoot(Directory.GetCurrentDirectory())
            //                        .UseStartup<Startup>()
            //                        .UseServiceFabricIntegration(listener, ServiceFabricIntegrationOptions.None)
            //                        .UseUrls(url)
            //                        .ConfigureAppConfiguration((hostingContext, config) =>
            //                        {
            //                            config
            //                                .SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
            //                                .AddJsonFile("appsettings.json", true, true)
            //                                .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", true, true)
            //                                .AddJsonFile("ocelot.json", false, false)
            //                                .AddEnvironmentVariables();
            //                        })
            //                        .ConfigureServices(s => {
            //                            s.AddOcelot();
            //                        })
            //                        .Configure(a => {
            //                            a.UseOcelot().Wait();
            //                        })
            //                        .Build();
            //        }))
            //};
        }

        private static void ConfigureSignalR()
        {
            //app.UseAesDataProtectorProvider(SignalRHostConfiguration.EncryptionPassword);

            //if (SignalRHostConfiguration.UseScaleout)
            //{
            //    var serviceBusConfig = new ServiceBusScaleoutConfiguration(SignalRHostConfiguration.ServiceBusConnectionString,
            //        SignalRHostConfiguration.ServiceBusBackplaneTopic);

            //    GlobalHost.DependencyResolver.UseServiceBus(serviceBusConfig);
            //    app.MapSignalR();
            //}

            //var configuration = new HubConfiguration { EnableDetailedErrors = true, EnableJavaScriptProxies = false };
            //app.MapSignalR(configuration);

            var connectionString = "Endpoint=sb://medicalbookingservicebus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=l547OSCiiTaAEby08Ma79cMLAGSflwNmcbAP8LCkwsg=";
            var queueName = "eventsqueue";

            var queueClient = new QueueClient(connectionString, queueName);

            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                // Maximum number of Concurrent calls to the callback `ProcessMessagesAsync`, set to 1 for simplicity.
                // Set it according to how many messages the application wants to process in parallel.
                MaxConcurrentCalls = 1,

                // Indicates whether MessagePump should automatically complete the messages after returning from User Callback.
                // False below indicates the Complete will be handled by the User Callback as in `ProcessMessagesAsync` below.
                AutoComplete = false
            };

            // Register the function that will process messages
            queueClient.RegisterMessageHandler(async (message, token) =>
            {
                // Process the message

              //  await new EventsGenratedEventHandler(new NotificationService()).HandleEventsGeneratedEvent(message);
                var t = message.SystemProperties.SequenceNumber;
               // var customer = JsonConvert.DeserializeObject<Event>(Encoding.UTF8.GetString(message.Body));
                //reList.Add(message.GetBody<string>());
                // Complete the message so that it is not received again.
                // This can be done only if the queueClient is opened in ReceiveMode.PeekLock mode.
                await queueClient.CompleteAsync(message.SystemProperties.LockToken);
            }, messageHandlerOptions);


        }

        static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            Console.WriteLine("Exception context for troubleshooting:");
            Console.WriteLine($"- Endpoint: {context.Endpoint}");
            Console.WriteLine($"- Entity Path: {context.EntityPath}");
            Console.WriteLine($"- Executing Action: {context.Action}");
            return Task.CompletedTask;
        }

    }
}
