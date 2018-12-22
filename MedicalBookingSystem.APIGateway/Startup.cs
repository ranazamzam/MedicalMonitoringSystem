using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Owin;
using Microsoft.Owin.Cors;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System.Text;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace MedicalBookingSystem.APIGateway
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        //public static void ConfigureApp(IAppBuilder app)
        //{
        //    ConfigureCors(app);
        //    ConfigureSignalR(app);
        //}

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddOcelot();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
            //app.UseCors(CorsOptions.AllowAll);
            app.UseOcelot().Wait();
            ConfigureSignalR();
        }

        private static void ConfigureCors(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);
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
            // Register the function that will process messages
            queueClient.RegisterMessageHandler(async (message, token) =>
            {
                // Process the message
                var t = message.SystemProperties.SequenceNumber;
              //  var customer = JsonConvert.DeserializeObject<Event>(Encoding.UTF8.GetString(message.Body));
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
