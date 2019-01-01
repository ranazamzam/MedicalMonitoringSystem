using MedicalBookingSystem.APIGateway.Aggregator.Config;
using MedicalBookingSystem.APIGateway.Aggregator.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MedicalBookingSystem.APIGateway.Aggregator
{
    public class PatientService
    {
        private readonly HttpClient _apiClient;

        public async Task<PatientData> GetById(string id)
        {
            var data = await _apiClient.GetStringAsync(_urls.Basket + UrlsConfig.BasketOperations.GetItemById(id));
            var patient = !string.IsNullOrEmpty(data) ? JsonConvert.DeserializeObject<PatientData>(data) : null;
            return patient;
        }
    }
}
