using Microsoft.AspNetCore.SignalR;
using System;

namespace MedicalBookingSystem.APIGateway.Aggregator.SignalRHub
{
    public class EventsGeneratorHub : Hub<IEventsGeneratorNotification>
    {
    }
}
