using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Owin;
using Microsoft.Owin.Cors;
using Microsoft.Azure.ServiceBus;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using EventBus.EventBusAzureServiceBus;
using EventBus.GenericEventBus.Interfaces;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using EventBus.GenericEventBus;
using SignalR;
using MedicalBookingSystem.APIGateway.Aggregator.Interfaces;
using MedicalBookingSystem.APIGateway.Aggregator;
using MedicalBookingSystem.APIGateway.Aggregator.Services;

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
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR();
            services.AddTransient<INotificationService, NotificationService>();
            services.AddTransient<IPatientService, PatientService>();
            services.AddTransient<IDoctorService, DoctorService>();
            services.AddTransient<IEventService, EventService>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddOcelot();
            services.AddIntegrationServices(Configuration);
            services.AddEventBus(Configuration);
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });

            var container = new ContainerBuilder();
            container.Populate(services);
            return new AutofacServiceProvider(container.Build());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }


            app.UseCors("CorsPolicy");
            app.UseSignalR(routes =>
            {
                routes.MapHub<EventsGeneratorHub>("/eventsGenerator");
            });


            app.UseMvc();
            //app.UseCors(CorsOptions.AllowAll);
            app.UseOcelot().Wait();

            ConfigureEventBus(app);
        }

        protected virtual void ConfigureEventBus(IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
            eventBus.Subscribe<MedicalBookingSystemGeneratedEventIntegrationEvent, MedicalBookingSystemGeneratedEventIntegrationEventHandler>();
        }

        private static void ConfigureCors(IAppBuilder app)
        {
            //app.UseCors(CorsOptions.AllowAll);
        }

        private static void ConfigureSignalR()
        {
            //var configuration = new HubConfiguration { EnableDetailedErrors = true, EnableJavaScriptProxies = false };
            //app.MapSignalR(configuration);

            //var connectionString = "Endpoint=sb://medicalbookingservicebus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=l547OSCiiTaAEby08Ma79cMLAGSflwNmcbAP8LCkwsg=";
            //var queueName = "eventsqueue";

            //var queueClient = new QueueClient(connectionString, queueName);

            //var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            //{
            //    // Maximum number of Concurrent calls to the callback `ProcessMessagesAsync`, set to 1 for simplicity.
            //    // Set it according to how many messages the application wants to process in parallel.
            //    MaxConcurrentCalls = 1,

            //    // Indicates whether MessagePump should automatically complete the messages after returning from User Callback.
            //    // False below indicates the Complete will be handled by the User Callback as in `ProcessMessagesAsync` below.
            //    AutoComplete = false
            //};

            //// Register the function that will process messages
            //// Register the function that will process messages
            //queueClient.RegisterMessageHandler(async (message, token) =>
            //{
            //    // Process the message
            //    var t = message.SystemProperties.SequenceNumber;
            //    //  var customer = JsonConvert.DeserializeObject<Event>(Encoding.UTF8.GetString(message.Body));
            //    //reList.Add(message.GetBody<string>());
            //    // Complete the message so that it is not received again.
            //    // This can be done only if the queueClient is opened in ReceiveMode.PeekLock mode.
            //    await queueClient.CompleteAsync(message.SystemProperties.LockToken);
            //}, messageHandlerOptions);


        }

    }

    public static class CustomExtensionMethods
    {
        public static IServiceCollection AddIntegrationServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = "Endpoint=sb://medicalbookingmonitorservicebus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=9j5ZjW9Y/BnphDca2o1nno23PMEhDy+nWJbTtuOz+CU=";
            var queueName = "eventsqueue";

            services.AddSingleton<IServiceBusPersisterConnection>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<DefaultServiceBusPersisterConnection>>();
                var serviceBusConnectionStringBuilder = new ServiceBusConnectionStringBuilder(connectionString);

                var serviceBusConnection = new ServiceBusConnectionStringBuilder(serviceBusConnectionStringBuilder.Endpoint, queueName, serviceBusConnectionStringBuilder.SasKeyName, serviceBusConnectionStringBuilder.SasKey);

                return new DefaultServiceBusPersisterConnection(serviceBusConnection, logger);
            });

            return services;
        }

        public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration)
        {
            var queueName = "eventsqueue";

            services.AddSingleton<IEventBus, EventBusServiceBus>(sp =>
            {
                var serviceBusPersisterConnection = sp.GetRequiredService<IServiceBusPersisterConnection>();
                var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
                var logger = sp.GetRequiredService<ILogger<EventBusServiceBus>>();
                var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

                return new EventBusServiceBus(serviceBusPersisterConnection, logger,
                                              eventBusSubcriptionsManager, queueName, iLifetimeScope);
            });

            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();
            services.AddTransient<MedicalBookingSystemGeneratedEventIntegrationEventHandler>();

            return services;
        }
    }
}
