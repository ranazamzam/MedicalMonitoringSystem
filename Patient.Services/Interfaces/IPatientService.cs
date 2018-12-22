using System.Collections.Generic;
using System.Threading.Tasks;

namespace Patient.Services.Interfaces
{
    public interface IPatientService
    {
        Task<Domain.Models.PatientEntity> GetPatientById(string Id);

        List<Domain.Models.PatientEntity> GetAllPatients();
    }
}
