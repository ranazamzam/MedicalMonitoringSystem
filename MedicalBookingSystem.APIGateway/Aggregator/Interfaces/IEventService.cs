using SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalBookingSystem.APIGateway.Aggregator.Interfaces
{
    public interface IEventService
    {
        Task<List<MedicalBookingSystemGeneratedEventIntegrationEvent>> GetEvents();
    }
}
