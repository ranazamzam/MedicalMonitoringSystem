using Microsoft.AspNetCore.SignalR;
using System;

namespace SignalR
{
    public class EventsGeneratorHub : Hub<IEventsGeneratorNotification>
    {
    }
}
