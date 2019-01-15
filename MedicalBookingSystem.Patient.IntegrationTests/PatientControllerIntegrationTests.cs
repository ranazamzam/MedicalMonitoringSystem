using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using NUnit.Framework;
using Patient.Domain.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MedicalBookingSystem.Patient.IntegrationTests
{
    [TestFixture]
    public class PatientControllerIntegrationTests
    {
        private readonly WebApplicationFactory<Startup> _factory;

        #region Constructor
        public PatientControllerIntegrationTests()
        {
            _factory = new WebApplicationFactory<Startup>();
        }
        #endregion

        [Test]
        public async Task GetAllPatients_ThereIsPatients_ShouldReturnOkAndListOfPatients()
        {
            var client = _factory.CreateClient();

            var httpResponse = await client.GetAsync("/api/Patient/Patients");

            //httpResponse.EnsureSuccessStatusCode();
            //var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            //var players = JsonConvert.DeserializeObject<IEnumerable<PatientDTO>>(stringResponse);
        }
    }
}
