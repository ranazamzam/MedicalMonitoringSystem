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
        public static async Task Run(
            [TimerTrigger("0 */1 * * * *")]TimerInfo myTimer,
            ILogger log, [ServiceBus("eventsqueue", Connection = "ServiceBusConnection")] ICollector<Event> eventQueueItem,
            [Table("EventsTable")] CloudTable currentEvents,
            [Table("EventsTable")] ICollector<Event> newEvents)
        {
            try
            {
                var patientsIds = Helper.GetPatients();
                var doctorsIds = Helper.GetDoctors();
                var eventTypes = Helper.GetEventTypes();

                // Generate a random event
                Random randomNumberGenerator = new Random();
                var randomEventTypeIndex = randomNumberGenerator.Next(eventTypes.Count);
                var eventType = eventTypes[randomEventTypeIndex];
                
                var randomPatientIdIndex = randomNumberGenerator.Next(patientsIds.Count);
                var patientId = patientsIds[randomPatientIdIndex];
                int? doctorId = null;

                if (eventType.Contains("Appointment"))
                {
                    var randomDoctorIdIndex = randomNumberGenerator.Next(doctorsIds.Count);
                    doctorId = doctorsIds[randomDoctorIdIndex];
                }

                // Genrate Random Date
                var IsAppointmentEvent = eventType.Contains("Booking Appointment");
                var eventDate = Helper.GenerateRandomDate(IsAppointmentEvent);
                var partitionKey = IsAppointmentEvent ? "BookingAppointmentEvent" : "GeneralEvent";

                var rowId = Guid.NewGuid();
                var generatedEvent = new Event
                {
                    RowKey = rowId.ToString(),
                    PartitionKey = partitionKey,
                    EventId = rowId,
                    EventReferenceNo = rowId.ToString().Substring(0, rowId.ToString().IndexOf("-")),
                    PatientId = patientId,
                    DoctorId = doctorId,
                    EventDate = eventDate,
                    EventCreationDate = DateTime.Now,
                    EventType = eventType,
                };

                if (eventType.Contains("Booking Appointment"))
                {
                    var originalAppointment = await Helper.CheckIfAppointmentIsConflicting(currentEvents, eventDate, doctorId.Value);
                    
                    if (originalAppointment != null)
                    {
                       // var originalAppointment = conflictedEvents.Results[0];
                        generatedEvent.IsConflicted = true;
                        generatedEvent.IsConflictShown = false;
                        generatedEvent.OriginalEventId = originalAppointment.EventId;
                    }
                }

                // Publish event to queue and save it to table storage
                eventQueueItem.Add(generatedEvent);
                newEvents.Add(generatedEvent);
            }
            catch (Exception ex)
            {
                log.LogInformation($"Event Generator Timer trigger function executed at: {DateTime.Now} exception:{ex.Message}");
            }
        }
    }

    #region Helper
    public class Helper
    {
        /// <summary>
        /// Returns the list of patients Ids 
        /// This list should be saved in a database
        /// </summary>
        /// <returns></returns>
        public static List<int> GetPatients()
        {
            return new List<int>()
            {
                1,
                2,
                3
            };
        }

        /// <summary>
        /// Returns the list of doctors Ids 
        /// This list should be saved in a database
        /// </summary>
        /// <returns></returns>
        public static List<int> GetDoctors()
        {
            return new List<int>()
            {
                1,
                2,
                3
            };
        }

        /// <summary>
        /// Returns the list of possibe event types we can have in our system
        /// This list should be saved in a database
        /// </summary>
        /// <returns></returns>
        public static List<string> GetEventTypes()
        {
            return new List<string>()
            {
                "Logging",
                "Viewing",
                "Booking Appointment",
                "Cancelling Appointment"
            };
        }

        public static DateTime GetRandomDate(DateTime from, DateTime to)
        {
            Random randomNumberGenerator = new Random();
            TimeSpan range = new TimeSpan(to.Ticks - from.Ticks);
            return from + new TimeSpan((long)(range.Ticks * randomNumberGenerator.NextDouble()));
        }

        /// <summary>
        /// This function generate a random date for each event
        /// In case the event is an appointment event , I will fix the date generated to raise up the probability of getting 
        /// a conflict faster for making testing easier
        /// </summary>
        /// <param name="IsAppointmentEvent"></param>
        /// <returns></returns>
        public static DateTime GenerateRandomDate(bool IsAppointmentEvent)
        {
            Random randomNumberGenerator = new Random();

            int randomMonthMin = 1;
            int randomMonthMax = 12;
            int randomHourMin = 1;
            int randomHourMax = 23;
            int randomDayMin = 1;

            if (IsAppointmentEvent)
            {
                randomMonthMax = randomMonthMin;
                randomHourMin = 10;
                randomHourMax = 11;
                randomDayMin = 4;
            }

            var randomYear = randomNumberGenerator.Next(2019, 2019);
            var randomMonth = randomNumberGenerator.Next(randomMonthMin, randomMonthMax);
            var daysInMonth = DateTime.DaysInMonth(randomYear, randomMonth);
            var randomHour = randomNumberGenerator.Next(randomHourMin, randomHourMax);
            var randomMinute = randomNumberGenerator.Next(1, 59);
            var randomSecond = randomNumberGenerator.Next(1, 59);

            var randomDay = randomNumberGenerator.Next(randomDayMin, IsAppointmentEvent ? randomDayMin : daysInMonth);

            DateTime randomDateTime = new DateTime(randomYear, randomMonth, randomDay, randomHour, randomMinute, randomSecond);

            return randomDateTime;
        }

        /// <summary>
        /// Check if the new generated appointment is conflicting with the any of the apppointments booked before 
        /// In case the generated event type is Booking Appointment , we will query the events table to check if there is any appointment conflicting with the new one
        /// I am assuming that every appointment will have a range of 30 minutes, so if any new appointment is booked within this range 
        /// it shoud be marked as a conflicted one, for example if we have an appointment at 17 , any appointment between 
        /// 17 and 17:30 will be marked as a conflicted one 
        /// </summary>
        /// <param name="currentEvents"></param>
        /// <param name="eventDate"></param>
        /// <param name="doctorId"></param>
        /// <returns></returns>
        public static async Task<Event> CheckIfAppointmentIsConflicting(CloudTable currentEvents, DateTime eventDate, int doctorId)
        {
            DateTimeOffset eventStartDateTimeOffset = new DateTimeOffset(eventDate);
            DateTimeOffset eventEndDateTimeOffset = new DateTimeOffset(eventDate).AddMinutes(-30);
            var partitionFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "BookingAppointmentEvent");
            var doctorFilter = TableQuery.GenerateFilterConditionForInt("DoctorId", QueryComparisons.Equal, doctorId);
            var isConflictedFilter = TableQuery.GenerateFilterConditionForBool("IsConflicted", QueryComparisons.Equal, false);
            var dateLessThanFilter = TableQuery.GenerateFilterConditionForDate("EventDate", QueryComparisons.LessThanOrEqual, eventStartDateTimeOffset);
            var dateGreaterThanFilter = TableQuery.GenerateFilterConditionForDate("EventDate", QueryComparisons.GreaterThanOrEqual, eventEndDateTimeOffset);

            var filter = TableQuery.CombineFilters(dateLessThanFilter, TableOperators.And,
                                                      TableQuery.CombineFilters(
                                                           TableQuery.CombineFilters(partitionFilter, TableOperators.And, doctorFilter),
                                                           TableOperators.And, isConflictedFilter));
            var finalFilter = TableQuery.CombineFilters(filter, TableOperators.And, dateGreaterThanFilter);

            TableQuery<Event> rangeQuery = new TableQuery<Event>().Where(finalFilter);
            var conflictedEvents = await currentEvents.ExecuteQuerySegmentedAsync(rangeQuery, null);

            if (conflictedEvents.Results.Count > 0)
            {
                return conflictedEvents.Results[0];
            }

            return null;
        }
    }

    #endregion

    #region Classes
    public class Event : TableEntity
    {
        public Guid EventId { get; set; }

        public string EventReferenceNo { get; set; }

        public int PatientId { get; set; }

        public int? DoctorId { get; set; }

        public DateTime EventDate { get; set; }

        public DateTime EventCreationDate { get; set; }

        public string EventType { get; set; }

        public bool IsConflicted { get; set; }

        public bool IsConflictShown { get; set; }

        // In case of conflict, this property will contain the Id of the original event
        public Guid OriginalEventId { get; set; }

        public string OriginalEventReferenceNo { get; set; }
    }

    public class AppointmentEvent : Event
    {
        public List<Event> ConflictedEvents { get; set; }
    }

    #endregion
}
