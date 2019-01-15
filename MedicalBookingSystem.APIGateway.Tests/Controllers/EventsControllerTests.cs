using MedicalBookingSystem.APIGateway.Aggregator.Interfaces;
using MedicalBookingSystem.APIGateway.Aggregator.Models;
using MedicalBookingSystem.APIGateway.Aggregator.SignalRHub;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MedicalBookingSystem.APIGateway.Tests.Controllers
{
    [TestFixture]
    public class EventsControllerTests
    {
        private static Mock<ILogger<EventsController>> _loggerMock;
        private Mock<IPatientService> _patientServiceMock;
        private Mock<IDoctorService> _doctorServiceMock;
        private Mock<IEventService> _eventServiceMock;
        private List<MedicalBookingSystemGeneratedEventIntegrationEvent> _events;
        private EventsController _eventsController;

        #region One time setup before all tests
        [OneTimeSetUp]
        public void Setup()
        {
            _events = SetUpEvents();
            _loggerMock = new Mock<ILogger<EventsController>>();
        }
        #endregion

        #region  setup before each test
        [SetUp]
        public void ReInitializeTest()
        {
            _events = SetUpEvents();
            _patientServiceMock = new Mock<IPatientService>();
            _doctorServiceMock = new Mock<IDoctorService>();
            _eventServiceMock = new Mock<IEventService>();
        }
        #endregion

        [Test]
        public void GetAllEvents_ThereIsEvents_ShouldReturnOkAndListOfEvents()
        {
            // Arrange
            _patientServiceMock.Setup(p => p.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(new PatientData { Id = 1, Name = "TestPatient" });
            _doctorServiceMock.Setup(p => p.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(new DoctorData { Id = 1, Name = "TestDoctor" });
            _eventServiceMock.Setup(p => p.GetEvents()).ReturnsAsync(_events);
            _eventsController = new EventsController(_eventServiceMock.Object, _patientServiceMock.Object, _doctorServiceMock.Object, _loggerMock.Object);

            // Act
            var response = _eventsController.GetAllEvents().GetAwaiter().GetResult();

            var okResult = response as OkObjectResult;

            // Assert
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var returnedEvents = okResult.Value as List<MedicalBookingSystemGeneratedEventIntegrationEvent>;
            Assert.IsNotNull(returnedEvents);
            Assert.IsInstanceOf(typeof(List<MedicalBookingSystemGeneratedEventIntegrationEvent>), returnedEvents);
            Assert.AreEqual(returnedEvents.Any(), true);

            Assert.AreEqual(2, returnedEvents.Count);
            Assert.AreEqual("abd", returnedEvents.First().EventReferenceNo);

        }

        [Test]
        public void GetAllPatients_IfThereIsNoEvents_ShouldReturnNotFoundAndEmptyList()
        {
            // Arrange
            _events.Clear();
            _patientServiceMock.Setup(p => p.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(new PatientData { Id = 1, Name = "TestPatient" });
            _doctorServiceMock.Setup(p => p.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(new DoctorData { Id = 1, Name = "TestDoctor" });
            _eventServiceMock.Setup(p => p.GetEvents()).ReturnsAsync(_events);
            _eventsController = new EventsController(_eventServiceMock.Object, _patientServiceMock.Object, _doctorServiceMock.Object, _loggerMock.Object);

            // Act
            var response = _eventsController.GetAllEvents().GetAwaiter().GetResult();

            var notFoundResult = response as NotFoundResult;

            // Assert
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }

        #region tear down after every test 

        [TearDown]
        public void DisposeTest()
        {
            _eventsController = null;
            _patientServiceMock = null;
            _doctorServiceMock = null;
            _eventServiceMock = null;
            _events = null;
        }
        #endregion

        #region One time tear down after all tests

        [OneTimeTearDown]
        public void DisposeAllObjects()
        {
            _events = null;
            _loggerMock = null;
        }
        #endregion

        #region Helpers and Data Initializer
        private List<MedicalBookingSystemGeneratedEventIntegrationEvent> SetUpEvents()
        {
            return new List<MedicalBookingSystemGeneratedEventIntegrationEvent>()
            {
                new MedicalBookingSystemGeneratedEventIntegrationEvent
                {
                    EventReferenceNo = "abd",
                    EventType = "booking"
                },
                new MedicalBookingSystemGeneratedEventIntegrationEvent
                {
                   EventReferenceNo = "abd",
                    EventType = "booking"
                },
            };
        }
        #endregion
    }
}
