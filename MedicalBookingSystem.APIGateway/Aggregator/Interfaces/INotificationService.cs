using SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalBookingSystem.APIGateway
{
    public interface INotificationService
    {
        Task BroadCastGeneratedEvent(MedicalBookingSystemGeneratedEventIntegrationEvent generatedEvent);
    }
}
