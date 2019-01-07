using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
        public static async Task Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer,
            [Table("EventsTable")] CloudTable currentEvents,
            [SignalR(HubName = "broadcastConflicts")] IAsyncCollector<SignalRMessage> signalRMessages, ILogger log)
        {
            try
            {
                log.LogInformation($"Appointments Conflicts Detector Timer trigger function executed at: {DateTime.Now}");

                DateTimeOffset dateTimeOffset = DateTimeOffset.Now.AddMinutes(-5);
                var partitionFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "BookingAppointmentEvent");
                var IsConflictedFilter = TableQuery.GenerateFilterConditionForBool("IsConflicted", QueryComparisons.Equal, true);
                var IsConflictShownFilter = TableQuery.GenerateFilterConditionForBool("IsConflictShown", QueryComparisons.Equal, false);
                var dateFilter = TableQuery.GenerateFilterConditionForDate("EventCreationDate", QueryComparisons.LessThanOrEqual, dateTimeOffset);

                var filter = TableQuery.CombineFilters(
                               dateFilter,
                               TableOperators.And,
                               TableQuery.CombineFilters(
                                    TableQuery.CombineFilters(partitionFilter, TableOperators.And, IsConflictedFilter),
                                    TableOperators.And, IsConflictShownFilter));

                TableQuery<Event> rangeQuery = new TableQuery<Event>().Where(filter);

                List<AppointmentEvent> appointmentEventsWithConflicts = new List<AppointmentEvent>();

                var conflictedEvents = await currentEvents.ExecuteQuerySegmentedAsync(rangeQuery, null);

                foreach (var conflictedEvent in conflictedEvents)
                {
                    log.LogInformation($"{conflictedEvent.PartitionKey}\t{conflictedEvent.RowKey}\t{conflictedEvent.Timestamp}\t{conflictedEvent.EventId}");

                    var originalAppointmentEvent = appointmentEventsWithConflicts.Find(x => x.EventId == conflictedEvent.OriginalEventId);

                    if (originalAppointmentEvent != null)
                    {
                        originalAppointmentEvent.ConflictedEvents.Add(conflictedEvent);
                    }
                    else
                    {
                        appointmentEventsWithConflicts.Add(new AppointmentEvent()
                        {
                            EventId = conflictedEvent.OriginalEventId,
                            EventReferansNo = conflictedEvent.OriginalEventId.ToString().Substring(0, conflictedEvent.OriginalEventId.ToString().IndexOf("-")),
                            ConflictedEvents = new List<Event>() { conflictedEvent }
                        });
                    }

                    conflictedEvent.IsConflictShown = true;
                    await currentEvents.ExecuteAsync(TableOperation.Replace(conflictedEvent));
                }

                if (appointmentEventsWithConflicts.Any())
                    await signalRMessages.AddAsync(new SignalRMessage
                    {
                        Target = "appointmentConflictDetected",
                        Arguments = new[] { appointmentEventsWithConflicts }
                    });
            }
            catch (Exception ex)
            {
                log.LogInformation($"AppointmentsConflictsDetector Timer trigger function executed at: {DateTime.Now} exception:{ex.Message}");
            }
        }
    }
}
