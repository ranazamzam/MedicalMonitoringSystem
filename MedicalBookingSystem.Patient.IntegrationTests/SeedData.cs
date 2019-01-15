using Patient.Domain.Models;
using Patient.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedicalBookingSystem.Patient.IntegrationTests
{
    public class SeedData
    {
        public static void PopulateTestData(PatientDbContext dbContext)
        {
            dbContext.Patients.AddRange(new PatientEntity { Id = 1, Name = "TestPatient1" },
                                        new PatientEntity { Id = 2, Name = "TestPatient2" });
            dbContext.SaveChanges();
        }
    }
}
