using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.WindowsAzure.Storage.Table;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MedicalBookingMonitoringSystemFunctionApp.Tests
{
    [TestFixture]
    public class AppointmentsConflictsDetectorTests
    {
        [Test]
        public void AppointmentsConflictsDetector_NoConflictedEvents_ShouldNotAddMessageToSignalRService()
        {
            // Arrange
            var logger = (ListLogger)TestFactory.CreateLogger(LoggerTypes.List);
            var currentEventsTable = TestFactory.GetClientForTable("EventsTestTable");
            var signalRMock = new Mock<IAsyncCollector<SignalRMessage>>();

            // Act
            AppointmentsConflictsDetector.Run(null, currentEventsTable, signalRMock.Object, logger).GetAwaiter().GetResult();

            // Assert
            signalRMock.Verify(m => m.AddAsync(It.IsAny<SignalRMessage>(), It.IsAny<CancellationToken>()), Times.Never());
        }

        [Test]
        public void AppointmentsConflictsDetector_ThereIsConflictedEvents_ShouldAddMessageToSignalRService()
        {
            // Arrange
            var logger = (ListLogger)TestFactory.CreateLogger(LoggerTypes.List);
            var currentEventsTable = TestFactory.GetClientForTable("EventsTestTable");
            var signalRMock = new Mock<IAsyncCollector<SignalRMessage>>();
            AddConflictedEventsToTestTable();

            // Act
            AppointmentsConflictsDetector.Run(null, currentEventsTable, signalRMock.Object, logger).GetAwaiter().GetResult();

            // Assert
            signalRMock.Verify(m => m.AddAsync(It.IsAny<SignalRMessage>(), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Test]
        public void AppointmentsConflictsDetector_ShouldNotThrowException()
        {
            // Arrange
            var logger = (ListLogger)TestFactory.CreateLogger(LoggerTypes.List);
            var currentEventsTable = TestFactory.GetClientForTable("EventsTestTable");
            var signalRMock = new Mock<IAsyncCollector<SignalRMessage>>();

            // Act & Assert
            Assert.DoesNotThrow(() => AppointmentsConflictsDetector.Run(null, currentEventsTable, signalRMock.Object, logger).GetAwaiter().GetResult());
        }

        [Test]
        public void AppointmentsConflictsDetector_ShoulLogMessageWhenStarted()
        {
            // Arrange
            var logger = (ListLogger)TestFactory.CreateLogger(LoggerTypes.List);
            var currentEventsTable = TestFactory.GetClientForTable("EventsTestTable");
            var signalRMock = new Mock<IAsyncCollector<SignalRMessage>>();

            // Act
            AppointmentsConflictsDetector.Run(null, currentEventsTable, signalRMock.Object, logger).GetAwaiter().GetResult();

            // Assert
            var msg = logger.Logs[0];
            Assert.AreEqual(true, msg.Contains("Appointments Conflicts Detector Timer trigger function executed at"));
        }

        #region Helpers

        public void AddConflictedEventsToTestTable()
        {
            var eventsTestTable = TestFactory.GetClientForTable("EventsTestTable");

            // Add non conficted event, then add a conflicted event to this one
            // The conflicted event is created with an old so that it s detected as a conflict as 5 minute should pass
            // before showing the event as conflicted
            var rowId = Guid.NewGuid();
            var nonConflictedTestEvent = new Event
            {
                RowKey = rowId.ToString(),
                PartitionKey = "BookingAppointmentEvent",
                EventId = rowId,
                PatientId = 1,
                DoctorId = 1,
                EventDate = DateTime.Now,
                EventCreationDate = DateTime.Now,
                EventType = "Booking Appointment",
                IsConflicted = false,
                IsConflictShown = false,
            };

            rowId = Guid.NewGuid();
            var conflictedTestEvent = new Event
            {
                RowKey = rowId.ToString(),
                PartitionKey = "BookingAppointmentEvent",
                EventId = rowId,
                PatientId = 2,
                DoctorId = 1,
                EventDate = DateTime.Now,
                EventCreationDate = DateTime.Now.AddMinutes(-6),
                EventType = "Booking Appointment",
                IsConflicted = true,
                IsConflictShown = false,
                OriginalEventId = nonConflictedTestEvent.EventId
            };

            TableBatchOperation batchOperation = new TableBatchOperation();
            batchOperation.Insert(nonConflictedTestEvent);
            batchOperation.Insert(conflictedTestEvent);

            eventsTestTable.ExecuteBatchAsync(batchOperation).GetAwaiter().GetResult();
        }
        #endregion
    }
}
