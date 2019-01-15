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

namespace MedicalBookingSystem.APIGateway.Aggregator.Services
{
    public class DoctorService : IDoctorService
    {
        #region Properties
        private readonly HttpClient _apiClient;
        private readonly StatelessServiceContext _serviceContext;
        #endregion

        public DoctorService(HttpClient apiClient, StatelessServiceContext serviceContext)
        {
            _apiClient = apiClient;
            _serviceContext = serviceContext;
        }

        /// <summary>
        /// Calls the doctor microservice API to get the name using the doctor Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<DoctorData> GetByIdAsync(int id)
        {
            var doctorServiceName = new Uri($"{_serviceContext.CodePackageActivationContext.ApplicationName}/MedicalBookingSystem.Doctor");
            var doctorServiceReverseProxyUrl = new Uri($"{APIGatewayConfiguration.ReverseProxyUri}{doctorServiceName.AbsolutePath}");

            var response = await _apiClient.GetAsync(doctorServiceReverseProxyUrl + UrlsConfig.DoctorOperations.GetDoctorById(id.ToString()));

            DoctorData doctor = null;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                doctor = JsonConvert.DeserializeObject<DoctorData>(content);
            }

            return doctor;
        }
    }
}
