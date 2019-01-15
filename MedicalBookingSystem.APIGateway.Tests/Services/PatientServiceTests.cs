using MedicalBookingSystem.APIGateway.Aggregator;
using MedicalBookingSystem.APIGateway.Aggregator.Config;
using MedicalBookingSystem.APIGateway.Aggregator.Interfaces;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using ServiceFabric.Mocks;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Tests
{
    [TestFixture]
    public class Tests
    {
        private IPatientService _patientService;
        private Mock<HttpClient> _mockHttpClient;

        #region  setup before each test
        [SetUp]
        public void ReInitializeTest()
        {
            _mockHttpClient = new Mock<HttpClient>();
        }
        #endregion

        [Test]
        public void GetByIdAsync_PatientIdExists_ShouldReturnPatientObject()
        {
            var context = MockStatelessServiceContextFactory.Default;
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock.Protected()
                       .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                       .ReturnsAsync(new HttpResponseMessage()
                       {
                           StatusCode = HttpStatusCode.OK,
                           Content = new StringContent("{'Id':1,'Name':'TestPatient'}"),
                       })
                       .Verifiable();
            var httpClient = new HttpClient(handlerMock.Object);
            _patientService = new PatientService(httpClient, context);

            var patient = _patientService.GetByIdAsync(1).GetAwaiter().GetResult();

            Assert.IsNotNull(patient);
            Assert.AreEqual(1, patient.Id);
            Assert.AreEqual("TestPatient", patient.Name);
        }

        [Test]
        public void GetByIdAsync_PatientIdNotExists_ShouldReturnNull()
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
            _patientService = new PatientService(httpClient, context);

            var patient = _patientService.GetByIdAsync(1).GetAwaiter().GetResult();

            Assert.IsNull(patient);
        }
    }
}