using EventBus.GenericEventBus.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace SignalR
{
    public class MedicalBookingSystemGeneratedEventIntegrationEvent : IntegrationEvent
    {
        public string PartitionKey { get; set; }

        public string RowKey { get; set; }

        public Guid EventId { get; set; }

        public int PatientId { get; set; }

        public int? DoctorId { get; set; }

        public DateTime EventDate { get; set; }

        public string EventType { get; set; }
    }
}
