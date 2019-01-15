using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage.Table;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalBookingMonitoringSystemFunctionApp.Tests
{
    public class EventsRetreiverTests
    {
        [Test]
        public async Task EventsRetreiverTests_WithPatientIdFilter_ShouldGetEventForThisPatient()
        {
            // Arrange
            var logger = (ListLogger)TestFactory.CreateLogger(LoggerTypes.List);
            var currentEventsTable = TestFactory.GetClientForTable("EventsTestTable");
            AddEventsForPatient3ToTestTable();

            // Act
            var request = TestFactory.CreateHttpRequest("patientId", "3");
            var response = (OkObjectResult)await EventsRetreiver.Run(request, currentEventsTable, logger);

            // Assert
            var okResult = response as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var returnedEvents = okResult.Value as List<Event>;
            Assert.IsNotNull(returnedEvents);
            Assert.IsInstanceOf(typeof(List<Event>), returnedEvents);
            Assert.AreEqual(returnedEvents.Any(), true);
            Assert.AreEqual(2, returnedEvents.Count);
        }

        [Test]
        public async Task EventsRetreiverTests_WithDoctorIdFilter_ShouldGetEventForThisDoctor()
        {
            // Arrange
            var logger = (ListLogger)TestFactory.CreateLogger(LoggerTypes.List);
            var currentEventsTable = TestFactory.GetClientForTable("EventsTestTable");
            AddEventsForDoctor1ToTestTable();

            // Act
            var request = TestFactory.CreateHttpRequest("doctorId", "1");
            var response = (OkObjectResult)await EventsRetreiver.Run(request, currentEventsTable, logger);

            // Assert
            var okResult = response as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var returnedEvents = okResult.Value as List<Event>;
            Assert.IsNotNull(returnedEvents);
            Assert.IsInstanceOf(typeof(List<Event>), returnedEvents);
            Assert.AreEqual(returnedEvents.Any(), true);
            Assert.AreEqual(1, returnedEvents.Count);
        }

        #region Helpers

        public void AddEventsForPatient3ToTestTable()
        {
            var eventsTestTable = TestFactory.GetClientForTable("EventsTestTable");

            // Delete rows of patient Id 3 if already exists
            TableQuery<Event> rangeQuery = new TableQuery<Event>();
            rangeQuery.Where(TableQuery.GenerateFilterConditionForInt("PatientId", QueryComparisons.Equal, 3));

            var existingEvents = eventsTestTable.ExecuteQuerySegmentedAsync(rangeQuery, null).GetAwaiter().GetResult();

            if (existingEvents.Any())
            {
                TableBatchOperation batchDeleteOperation = new TableBatchOperation();

                foreach (var item in existingEvents)
                {
                    batchDeleteOperation.Delete(item);
                }

                eventsTestTable.ExecuteBatchAsync(batchDeleteOperation).GetAwaiter().GetResult();
            }
            // Insert 2 events for patient Id = 3
            var rowId = Guid.NewGuid();
            var testEvent1 = new Event
            {
                RowKey = rowId.ToString(),
                PartitionKey = "BookingAppointmentEvent",
                EventId = rowId,
                PatientId = 3,
                DoctorId = 1,
                EventDate = DateTime.Now,
                EventCreationDate = DateTime.Now,
                EventType = "Booking Appointment",
                IsConflicted = false,
                IsConflictShown = false,
            };

            rowId = Guid.NewGuid();
            var testEvent2 = new Event
            {
                RowKey = rowId.ToString(),
                PartitionKey = "BookingAppointmentEvent",
                EventId = rowId,
                PatientId = 3,
                DoctorId = 1,
                EventDate = DateTime.Now,
                EventCreationDate = DateTime.Now.AddMinutes(-6),
                EventType = "Booking Appointment",
                IsConflicted = false,
                IsConflictShown = false
            };

            TableBatchOperation batchInsertOperation = new TableBatchOperation();
            batchInsertOperation.Insert(testEvent1);
            batchInsertOperation.Insert(testEvent2);

            eventsTestTable.ExecuteBatchAsync(batchInsertOperation).GetAwaiter().GetResult();
        }

        public void AddEventsForDoctor1ToTestTable()
        {
            var eventsTestTable = TestFactory.GetClientForTable("EventsTestTable");

            // Delete rows of doctor Id 1 if already exists
            TableQuery<Event> rangeQuery = new TableQuery<Event>();
            rangeQuery.Where(TableQuery.GenerateFilterConditionForInt("DoctorId", QueryComparisons.Equal, 1));

            var existingEvents = eventsTestTable.ExecuteQuerySegmentedAsync(rangeQuery, null).GetAwaiter().GetResult();

            if (existingEvents.Any())
            {
                TableBatchOperation batchDeleteOperation = new TableBatchOperation();

                foreach (var item in existingEvents)
                {
                    batchDeleteOperation.Delete(item);
                }

                eventsTestTable.ExecuteBatchAsync(batchDeleteOperation).GetAwaiter().GetResult();
            }
            // Insert 2 events for patient Id = 3
            var rowId = Guid.NewGuid();
            var testEvent1 = new Event
            {
                RowKey = rowId.ToString(),
                PartitionKey = "BookingAppointmentEvent",
                EventId = rowId,
                PatientId = 3,
                DoctorId = 1,
                EventDate = DateTime.Now,
                EventCreationDate = DateTime.Now,
                EventType = "Booking Appointment",
                IsConflicted = false,
                IsConflictShown = false,
            };

            TableBatchOperation batchInsertOperation = new TableBatchOperation();
            batchInsertOperation.Insert(testEvent1);
            eventsTestTable.ExecuteBatchAsync(batchInsertOperation).GetAwaiter().GetResult();
        }
        #endregion
    }
}
