using Microsoft.AspNetCore.SignalR;
using SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalBookingSystem.APIGateway
{
    public class NotificationService : INotificationService
    {
        private IHubContext<EventsGeneratorHub, IEventsGeneratorNotification> _hubContext;

        private IHubContext<EventsGeneratorHub> _hubContext1;
        public NotificationService(IHubContext<EventsGeneratorHub, IEventsGeneratorNotification> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task BroadCastGeneratedEvent(Event generatedEvent)
        {
            await _hubContext.Clients.All.ReceiveNewEvent(generatedEvent);
        }
    }
}
