using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalBookingSystem.APIGateway.Aggregator.Config
{
    public class UrlsConfig
    {
        // The operations to be called on the Patient microservices
        public class PatientOperations
        {
            public static string GetPatientById(string id) => $"/api/Patient/{id}";
        }

        // The operations to be called on the Doctor microservices
        public class DoctorOperations
        {
            public static string GetDoctorById(string id) => $"/api/Doctor/{id}";
        }
    }
}
