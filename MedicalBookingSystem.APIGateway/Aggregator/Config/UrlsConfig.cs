using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalBookingSystem.APIGateway.Aggregator.Config
{
    public class UrlsConfig
    {
        public class PatientOperations
        {
            public static string GetPatientById(string id) => $"/api/Patient/{id}";
        }

        public class DoctorOperations
        {
            public static string GetDoctorById(string id) => $"/api/Doctor/{id}";
        }
    }
}
