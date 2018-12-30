using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;

namespace MedicalBookingMonitoringSystemFunctionApp
{
    public static class AppointmentsConflictsDetector
    {
        [FunctionName("Function1")]
        public static Task Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer,
            [Table("EventsTable")] CloudTable currentEvents,
            [SignalR(HubName = "broadcastConflicts")] IAsyncCollector<SignalRMessage> signalRMessages,ILogger log)
        {
            var rowId = Guid.NewGuid();
            var generatedEvent = new Event
            {
                RowKey = rowId.ToString(),
                PartitionKey = "GeneralEvents",
                EventId = rowId,
                PatientId = 1,
                DoctorId = 1,
                EventDate = DateTime.Now,
                EventType = "login",
                Label = "MedicalBookingSystemGeneratedEvent",
            };

            
            var generatedEvent1 = new Event
            {
                RowKey = rowId.ToString(),
                PartitionKey = "GeneralEvents",
                EventId = rowId,
                PatientId = 2,
                DoctorId = 1,
                EventDate = DateTime.Now,
                EventType = "login",
                Label = "MedicalBookingSystemGeneratedEvent",
            };

            var rowId1 = Guid.NewGuid();
            var generatedEvent2 = new Event
            {
                RowKey = rowId1.ToString(),
                PartitionKey = "GeneralEvents",
                EventId = rowId,
                PatientId = 1,
                DoctorId = 1,
                EventDate = DateTime.Now,
                EventType = "login",
                Label = "MedicalBookingSystemGeneratedEvent",
            };


            var generatedEvent3 = new Event
            {
                RowKey = rowId1.ToString(),
                PartitionKey = "GeneralEvents",
                EventId = rowId,
                PatientId = 2,
                DoctorId = 1,
                EventDate = DateTime.Now,
                EventType = "login",
                Label = "MedicalBookingSystemGeneratedEvent",
            };
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            return signalRMessages.AddAsync(
                new SignalRMessage
                {
                    Target = "appointmentConflictDetected",
                    Arguments = new[] { new List<Event> { generatedEvent, generatedEvent1, generatedEvent2, generatedEvent3 } }
                });
        }
    }
}
