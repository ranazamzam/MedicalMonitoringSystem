﻿using EventBus.GenericEventBus.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedicalBookingSystem.APIGateway.Aggregator.SignalRHub
{
    /// <summary>
    /// Represents the properties of an event
    /// </summary>
    public class MedicalBookingSystemGeneratedEventIntegrationEvent : IntegrationEvent
    {
        public Guid EventId { get; set; }

        public string EventReferenceNo { get; set; }

        public int PatientId { get; set; }

        public int? DoctorId { get; set; }

        public string PatientName { get; set; }

        public string DoctorName { get; set; }

        public DateTime EventDate { get; set; }

        /// <summary>
        /// Event Date as string for fixing time zone 's issue
        /// </summary>
        public string EventDateStr { get; set; }

        public string EventType { get; set; }

        public bool IsConflictShown { get; set; }

        /// <summary>
        /// In case of a conflicetd event, this property will hold the reference no of the original event
        /// e.g The reference no of the first appointment booked at the same time with the same doctor
        /// </summary>
        public string OriginalEventReferenceNo { get; set; }
    }
}
