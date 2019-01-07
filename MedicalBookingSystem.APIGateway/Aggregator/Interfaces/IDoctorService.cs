using MedicalBookingSystem.APIGateway.Aggregator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalBookingSystem.APIGateway.Aggregator.Interfaces
{
    public interface IDoctorService
    {
        Task<DoctorData> GetByIdAsync(int id);
    }
}
