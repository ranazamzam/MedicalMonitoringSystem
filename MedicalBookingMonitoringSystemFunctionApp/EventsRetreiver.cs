using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;
using System.Linq;

namespace MedicalBookingMonitoringSystemFunctionApp
{
    public static class EventsRetreiver
    {
        /// <summary>
        /// This is function is used to get all already saved events and return them to the caller
        /// It can receive PatientId or DoctorId as query string to filter events accoring to the sent query string 
        /// If no query string is sent , all events are returned
        /// </summary>
        /// <param name="req"></param>
        /// <param name="currentEvents"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName("EventsRetreiver")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            [Table("EventsTable")] CloudTable currentEvents,
            ILogger log)
        {
            try
            {
                log.LogInformation("C# HTTP trigger function processed a request.");

                string patientIdQueryString = req.Query["patientId"];
                string doctorIdQueryString = req.Query["doctorId"];

                int patientId = 0;
                int doctorId = 0;

                if (!string.IsNullOrEmpty(patientIdQueryString))
                    int.TryParse(patientIdQueryString, out patientId);

                if (!string.IsNullOrEmpty(doctorIdQueryString))
                    int.TryParse(doctorIdQueryString, out doctorId);

                var patientFilter = string.Empty;
                var doctorFilter = string.Empty;

                if (patientId != 0)
                {
                    patientFilter = TableQuery.GenerateFilterConditionForInt("PatientId", QueryComparisons.Equal, patientId);
                }

                if (doctorId != 0)
                {
                    doctorFilter = TableQuery.GenerateFilterConditionForInt("DoctorId", QueryComparisons.Equal, doctorId);
                }

                var finalFilter = string.Empty;

                if (!string.IsNullOrEmpty(patientFilter) && !string.IsNullOrEmpty(doctorFilter))
                {
                    finalFilter = TableQuery.CombineFilters(patientFilter, TableOperators.And, doctorFilter);
                }
                else if (!string.IsNullOrEmpty(patientFilter))
                {
                    finalFilter = patientFilter;
                }
                else if (!string.IsNullOrEmpty(doctorFilter))
                {
                    finalFilter = doctorFilter;
                }

                TableQuery<Event> rangeQuery = new TableQuery<Event>();

                if (!string.IsNullOrEmpty(finalFilter))
                    rangeQuery.Where(finalFilter);

                var existingEvents = await currentEvents.ExecuteQuerySegmentedAsync(rangeQuery, null);
                List<Event> events = new List<Event>();

                foreach (var item in existingEvents)
                {
                    events.Add(new Event()
                    {
                        EventId = item.EventId,
                        EventReferenceNo = item.EventReferenceNo,
                        PatientId = item.PatientId,
                        DoctorId = item.DoctorId,
                        EventDate = item.EventDate,
                        EventDateStr = item.EventDateStr,
                        EventType = item.EventType,
                        IsConflictShown = item.IsConflictShown,
                        OriginalEventReferenceNo = item.OriginalEventId != Guid.Empty ? item.OriginalEventId.ToString().Substring(0, item.OriginalEventId.ToString().IndexOf("-")) : string.Empty,
                    });
                }

                return events.Any()
                    ? (ActionResult)new OkObjectResult(events)
                    : new NotFoundResult();

            }
            catch (Exception ex)
            {
                log.LogInformation($"EventsRetreiver trigger function executed at: {DateTime.Now} exception:{ex.Message}");
                return new BadRequestResult();
            }

        }
    }
}
