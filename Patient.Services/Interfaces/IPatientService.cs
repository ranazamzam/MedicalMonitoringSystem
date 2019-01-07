using Patient.Domain.DataTransferObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Patient.Services.Interfaces
{
    public interface IPatientService
    {
        PatientDTO GetPatientById(int id);

        List<PatientDTO> GetAllPatients();
    }
}
