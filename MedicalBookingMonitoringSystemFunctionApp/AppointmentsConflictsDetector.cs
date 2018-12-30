using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace MedicalBookingMonitoringSystemFunctionApp
{
    public static class AppointmentsConflictsDetector
    {
        [FunctionName("Function1")]
        public static Task Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer,
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
                PatientId = 1,
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
                    Arguments = new[] { generatedEvent,generatedEvent1 }
                });
        }
    }
}
