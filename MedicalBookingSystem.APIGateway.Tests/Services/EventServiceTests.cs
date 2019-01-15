using MedicalBookingSystem.APIGateway.Aggregator;
using MedicalBookingSystem.APIGateway.Aggregator.Config;
using MedicalBookingSystem.APIGateway.Aggregator.Interfaces;
using MedicalBookingSystem.APIGateway.Aggregator.Services;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using ServiceFabric.Mocks;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MedicalBookingSystem.APIGateway.Tests
{
    [TestFixture]
    public class EventServiceTests
    {
        private IEventService _eventService;
        private Mock<HttpClient> _mockHttpClient;

        #region  setup before each test
        [SetUp]
        public void ReInitializeTest()
        {
            _mockHttpClient = new Mock<HttpClient>();
        }
        #endregion

        [Test]
        public void GetEvents_EventsExists_ShouldReturnListOfEvents()
        {
            var context = MockStatelessServiceContextFactory.Default;
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock.Protected()
                       .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                       .ReturnsAsync(new HttpResponseMessage()
                       {
                           StatusCode = HttpStatusCode.OK,
                           Content = new StringContent("[{'EventReferenceNo':'abd','EventType':'Booking'},{'EventReferenceNo':'der','EventType':'Logging'}]"),
                       })
                       .Verifiable();
            var httpClient = new HttpClient(handlerMock.Object);
            _eventService = new EventService(httpClient);

            var @event = _eventService.GetEvents().GetAwaiter().GetResult();

            Assert.IsNotNull(@event);
            Assert.AreEqual(2, @event.Count);
        }

        [Test]
        public void GetEventsc_EventsNotExists_ShouldReturnNull()
        {
            var context = MockStatelessServiceContextFactory.Default;
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock.Protected()
                       .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                       .ReturnsAsync(new HttpResponseMessage()
                       {
                           StatusCode = HttpStatusCode.NotFound
                       })
                       .Verifiable();
            var httpClient = new HttpClient(handlerMock.Object);
            _eventService = new EventService(httpClient);

            var events = _eventService.GetEvents().GetAwaiter().GetResult();

            Assert.IsNull(events);
        }
    }
}
