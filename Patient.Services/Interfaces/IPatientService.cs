using Patient.Domain.DataTransferObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Patient.Services.Interfaces
{
    public interface IPatientService
    {
        Task<Domain.Models.PatientEntity> GetPatientByIdAsync(int Id);

        Domain.Models.PatientEntity GetPatientById(int id);

        List<PatientDTO> GetAllPatients();
    }
}
