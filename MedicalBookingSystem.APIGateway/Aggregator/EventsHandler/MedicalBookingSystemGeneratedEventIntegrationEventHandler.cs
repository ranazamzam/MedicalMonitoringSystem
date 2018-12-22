using EventBus.GenericEventBus.Interfaces;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MedicalBookingSystem.APIGateway
{
    public class MedicalBookingSystemGeneratedEventIntegrationEventHandler : IIntegrationEventHandler<MedicalBookingSystemGeneratedEventIntegrationEvent>
    {
        private INotificationService _notificationService;

        public MedicalBookingSystemGeneratedEventIntegrationEventHandler(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task Handle(MedicalBookingSystemGeneratedEventIntegrationEvent @event)
        {

            // Get patient name and doctor name from services

            // Notify connected clients of the new events
            await _notificationService.BroadCastGeneratedEvent(@event);
        }

        //public async Task HandleNewEventGeneratedEvent(Message message)
        //{
        //    // Process the message
        //    var t = message.SystemProperties.SequenceNumber;
        //    var generatedEvent = JsonConvert.DeserializeObject<Event>(Encoding.UTF8.GetString(message.Body));


        //}
    }
}
