using MedicalBookingSystem.APIGateway.Aggregator.Config;
using MedicalBookingSystem.APIGateway.Aggregator.Interfaces;
using MedicalBookingSystem.APIGateway.Aggregator.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace MedicalBookingSystem.APIGateway.Aggregator
{
    public class PatientService :IPatientService
    {
        private readonly HttpClient _apiClient;
        private readonly StatelessServiceContext _serviceContext;

        public PatientService(HttpClient apiClient, StatelessServiceContext serviceContext)
        {
            _apiClient = apiClient;
            _serviceContext = serviceContext;
        }

        public async Task<PatientData> GetByIdAsync(int id)
        {
            var patientServiceName = new Uri($"{_serviceContext.CodePackageActivationContext.ApplicationName}/MedicalBookingSystem.Patient");
            var patientServiceReverseProxyUrl = new Uri($"{APIGatewayConfiguration.ReverseProxyUri}{patientServiceName.AbsolutePath}");

            var response = await _apiClient.GetAsync(patientServiceReverseProxyUrl + UrlsConfig.PatientOperations.GetPatientById(id.ToString()));

            PatientData patient = null;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                patient = JsonConvert.DeserializeObject<PatientData>(content);
            }

            //var patient = !string.IsNullOrEmpty(data.) ? JsonConvert.DeserializeObject<PatientData>(data) : null;
            return patient;
        }
    }
}
