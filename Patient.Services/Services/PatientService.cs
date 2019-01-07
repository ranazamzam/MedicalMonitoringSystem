using Patient.Domain.DataTransferObjects;
using Patient.Domain.Interfaces;
using Patient.Domain.Models;
using Patient.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PatientEntity = Patient.Domain.Models.PatientEntity;

namespace Patient.Services.Services
{
    public class PatientService : IPatientService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<PatientEntity> _patientRepository;

        //public PatientService(IUnitOfWork unitOfWork)
        //{
        //    _unitOfWork = unitOfWork;
        //}

        public PatientService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Dispose()
        {
            _unitOfWork.Dispose();
        }

        public List<PatientDTO> GetAllPatients()
        {
            //var patients = _patientRepository.GetAll;
            //var patientsDTO = patients.Select(x => new PatientDTO()
            //{
            //    Id = x.Id,
            //    Name = x.Name
            //}).ToList();

            //patientsDTO.Insert(0, new PatientDTO { Id = -1, Name = "All" });

            //return patientsDTO;

            var patients = _unitOfWork.Repository<PatientEntity>().GetAllNoTracking.ToList();
            var patientsDTO = patients.Select(x => new PatientDTO()
            {
                Id = x.Id,
                Name = x.Name
            }).ToList();

            if (patientsDTO.Any())
                patientsDTO.Insert(0, new PatientDTO { Id = -1, Name = "All" });

            return patientsDTO;
        }

        public PatientDTO GetPatientById(int id)
        {
            var patient =  _unitOfWork.Repository<PatientEntity>().GetById(id);

            if (patient != null)
                return new PatientDTO()
                {
                    Id = patient.Id,
                    Name = patient.Name
                };

            return null;
        }
    }
}
