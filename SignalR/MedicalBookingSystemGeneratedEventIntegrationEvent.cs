using EventBus.GenericEventBus.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace SignalR
{
    public class MedicalBookingSystemGeneratedEventIntegrationEvent : IntegrationEvent
    {
        public Guid EventId { get; set; }

        public string EventReferansNo { get; set; }

        public int PatientId { get; set; }

        public int? DoctorId { get; set; }

        public string PatientName { get; set; }

        public string DoctorName { get; set; }

        public DateTime EventDate { get; set; }

        public string EventType { get; set; }

        public bool IsConflictShown { get; set; }
    }
}
