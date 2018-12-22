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

        public PatientService(IRepository<PatientEntity> patientRepository)
        {
            _patientRepository = patientRepository;
        }

        public void Dispose()
        {
            _unitOfWork.Dispose();
        }

        public List<PatientEntity> GetAllPatients()
        {
            return _patientRepository.GetAll.ToList();
           // return _unitOfWork.Repository<Domain.Models.Patient>().GetAllNoTracking.ToList();
        }

        public Task<PatientEntity> GetPatientById(string id)
        {
            return _patientRepository.GetByIdAsync(id);
            //return Task.FromResult(_unitOfWork.Repository<Domain.Models.Patient>().GetById(id));
        }
    }
}
