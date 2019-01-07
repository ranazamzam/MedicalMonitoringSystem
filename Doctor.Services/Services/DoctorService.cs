using Doctor.Domain.DataTransferObjects;
using Doctor.Domain.Interfaces;
using Doctor.Domain.Models;
using Doctor.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PatientEntity = Doctor.Domain.Models.DoctorEntity;

namespace Doctor.Services.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly IUnitOfWork _unitOfWork;


        public DoctorService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Dispose()
        {
            _unitOfWork.Dispose();
        }

        public List<DoctorDTO> GetAllDoctors()
        {
            var doctors = _unitOfWork.Repository<DoctorEntity>().GetAllNoTracking.ToList();
            var doctorsDTO = doctors.Select(x => new DoctorDTO()
            {
                Id = x.Id,
                Name = x.Name
            }).ToList();

            if (doctorsDTO.Any())
                doctorsDTO.Insert(0, new DoctorDTO { Id = -1, Name = "All" });

            return doctorsDTO;
        }

        public DoctorDTO GetDoctorById(int id)
        {
            var patient = _unitOfWork.Repository<DoctorEntity>().GetById(id);

            if (patient != null)
                return new DoctorDTO()
                {
                    Id = patient.Id,
                    Name = patient.Name
                };

            return null;
        }
    }
}
