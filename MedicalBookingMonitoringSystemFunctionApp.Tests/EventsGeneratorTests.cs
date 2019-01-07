using Microsoft.Azure.WebJobs;
using Moq;
using NUnit.Framework;
using System;

namespace MedicalBookingMonitoringSystemFunctionApp.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void GenerateEvent_ShouldCallAddToQueueOnlyOnce()
        {
            // Arrange
            var logger = (ListLogger)TestFactory.CreateLogger(LoggerTypes.List);
            var currentEventsTable = TestFactory.GetClientForTable("EventsTestTable");
            var eventsQueueMock = new Mock<ICollector<Event>>();
            var newEventsTableMock = new Mock<ICollector<Event>>();

            // Act
            EventsGenerator.Run(null, logger, eventsQueueMock.Object, currentEventsTable, newEventsTableMock.Object).GetAwaiter().GetResult();

            // Assert
            eventsQueueMock.Verify(m => m.Add(It.IsAny<Event>()), Times.Once());
        }

        [Test]
        public void GenerateEvent_ShouldCallAddToTableOnlyOnce()
        {
            // Arrange
            var logger = (ListLogger)TestFactory.CreateLogger(LoggerTypes.List);
            var currentEventsTable = TestFactory.GetClientForTable("EventsTestTable");
            var eventsQueueMock = new Mock<ICollector<Event>>();
            var newEventsTableMock = new Mock<ICollector<Event>>();

            // Act
            EventsGenerator.Run(null, logger, eventsQueueMock.Object, currentEventsTable, newEventsTableMock.Object).GetAwaiter().GetResult();

            // Assert
            newEventsTableMock.Verify(m => m.Add(It.IsAny<Event>()), Times.Once());
           // Assert.Pass();
        }

        [Test]
        public void GenerateEvent_ShouldNotThrowException()
        {
            // Arrange
            var logger = (ListLogger)TestFactory.CreateLogger(LoggerTypes.List);
            var currentEventsTable = TestFactory.GetClientForTable("EventsTestTable");
            var eventsQueueMock = new Mock<ICollector<Event>>();
            var newEventsTableMock = new Mock<ICollector<Event>>();

            // Act & Assert
            Assert.DoesNotThrow(() => EventsGenerator.Run(null, logger, eventsQueueMock.Object, currentEventsTable, newEventsTableMock.Object).GetAwaiter().GetResult());
        }
    }
}