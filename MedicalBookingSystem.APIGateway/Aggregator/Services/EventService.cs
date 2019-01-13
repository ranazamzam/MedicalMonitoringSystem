using MedicalBookingSystem.APIGateway.Aggregator.Config;
using MedicalBookingSystem.APIGateway.Aggregator.Interfaces;
using MedicalBookingSystem.APIGateway.Aggregator.SignalRHub;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace MedicalBookingSystem.APIGateway.Aggregator.Services
{
    public class EventService  : IEventService
    {
        private readonly HttpClient _apiClient;

        public EventService(HttpClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<List<MedicalBookingSystemGeneratedEventIntegrationEvent>> GetEvents()
        {
           var eventsAzureFunctionUrl = new Uri($"{APIGatewayConfiguration.EventsAzureFunctionUrl}");

            var response = await _apiClient.GetAsync(eventsAzureFunctionUrl);

            List<MedicalBookingSystemGeneratedEventIntegrationEvent> events = null;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                events = JsonConvert.DeserializeObject<List<MedicalBookingSystemGeneratedEventIntegrationEvent>>(content);
            }

            return events;
        }
    }
}
