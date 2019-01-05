using MedicalBookingSystem.APIGateway.Aggregator.Interfaces;
using Microsoft.AspNetCore.SignalR;
using SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalBookingSystem.APIGateway.Aggregator
{
    public class NotificationService : INotificationService
    {
        private IHubContext<EventsGeneratorHub, IEventsGeneratorNotification> _hubContext;

        public NotificationService(IHubContext<EventsGeneratorHub, IEventsGeneratorNotification> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task BroadCastGeneratedEvent(MedicalBookingSystemGeneratedEventIntegrationEvent generatedEvent)
        {
            await _hubContext.Clients.All.ReceiveNewEvent(generatedEvent);
        }
    }
}
