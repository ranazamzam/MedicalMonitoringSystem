using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;

namespace MedicalBookingMonitoringSystemFunctionApp
{
    public static class EventsGenerator
    {
        [FunctionName("EventsGenerator")]
        public static void Run(
            [TimerTrigger("0 */1 * * * *")]TimerInfo myTimer,
            ILogger log, [ServiceBus("eventsqueue", Connection = "ServiceBusConnection")] ICollector<Event> eventQueueItem,
            [Table("EventsTable")] CloudTable currentEvents,
            [Table("EventsTable")] ICollector<Event> newEvents)
        {
            var rowId = Guid.NewGuid();
            var generatedEvent = new Event
            {
                RowKey = rowId.ToString(),
                PartitionKey = "BookingAppointmentEvent",
                EventId = rowId,
                PatientId = 1,
                DoctorId = 1,
                EventDate = DateTime.Now,
                EventType = "login",
                Label = "MedicalBookingSystemGeneratedEvent",
            };
            rowId = Guid.NewGuid();
            var generatedEvent1 = new Event
            {
                RowKey = rowId.ToString(),
                PartitionKey = "BookingAppointmentEvent",
                EventId = rowId,
                PatientId = 1,
                DoctorId = 1,
                EventDate = DateTime.Now,
                EventType = "login",
                Label = "MedicalBookingSystemGeneratedEvent",
                IsConflicted = true,
                IsConflictShown = false,
                OriginalEventId = generatedEvent.EventId
            };
            rowId = Guid.NewGuid();
            var generatedEvent2 = new Event
            {
                RowKey = rowId.ToString(),
                PartitionKey = "BookingAppointmentEvent",
                EventId = rowId,
                PatientId = 1,
                DoctorId = 1,
                EventDate = DateTime.Now,
                EventType = "login",
                Label = "MedicalBookingSystemGeneratedEvent",
                IsConflicted = true,
                IsConflictShown = false,
                OriginalEventId = generatedEvent.EventId
            };

            eventQueueItem.Add(generatedEvent);

             newEvents.Add(generatedEvent);
            newEvents.Add(generatedEvent1);
            newEvents.Add(generatedEvent2);
            //  return (ActionResult)new OkObjectResult($"Ok");
        }
    }

    public class Event : TableEntity
    {
        public Guid EventId { get; set; }

        public int PatientId { get; set; }

        public int? DoctorId { get; set; }

        public DateTime EventDate { get; set; }

        public string EventType { get; set; }

        // Thi is used for being able to get the event name to handle the message when consuming it
        public string Label { get; set; }

        public bool IsConflicted { get; set; }

        public bool IsConflictShown { get; set; }

        // In case of conflict, this property will contain the Id of the original event
        public Guid OriginalEventId { get; set; }
    }

    public class AppointmentEvent : Event
    {
        public List<Event> ConflictedEvents { get; set; }
    }
}
