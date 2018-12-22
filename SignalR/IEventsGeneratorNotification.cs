﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SignalR
{
    public interface IEventsGeneratorNotification
    {
        Task ReceiveNewEvent(Event message);
    }
}
