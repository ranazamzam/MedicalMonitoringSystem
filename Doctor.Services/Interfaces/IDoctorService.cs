using Doctor.Domain.DataTransferObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Doctor.Services.Interfaces
{
    public interface IDoctorService
    {
        DoctorDTO GetDoctorById(int id);

        List<DoctorDTO> GetAllDoctors();
    }
}
