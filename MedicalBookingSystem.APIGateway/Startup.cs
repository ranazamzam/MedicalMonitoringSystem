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
using MedicalBookingSystem.APIGateway.Aggregator.Interfaces;
using MedicalBookingSystem.APIGateway.Aggregator;
using MedicalBookingSystem.APIGateway.Aggregator.Services;
using MedicalBookingSystem.APIGateway.Aggregator.SignalRHub;

namespace MedicalBookingSystem.APIGateway
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        
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
    }

    public static class CustomExtensionMethods
    {
        public static IServiceCollection AddIntegrationServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = "Endpoint=sb://medicalsystemservicebus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=NZRk50slK3s9vtZF4qZLsEhjCO0EWfKzOgYirBfP474=";
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
