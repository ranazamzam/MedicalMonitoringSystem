using MedicalBookingSystem.APIGateway.Aggregator.SignalRHub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalBookingSystem.APIGateway.Aggregator.Interfaces
{
    public interface INotificationService
    {
        Task BroadCastGeneratedEvent(MedicalBookingSystemGeneratedEventIntegrationEvent generatedEvent);
    }
}
