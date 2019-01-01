using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Patient.Domain.DataTransferObjects;
using Patient.Domain.Models;
using Patient.Services.Interfaces;


namespace MedicalBookingSystem.Patient.Controllers
{
    [Route("api/[controller]")]
    public class PatientController : Controller
    {
        private readonly IPatientService _patientService;

        public PatientController(IPatientService patientService)
        {
            _patientService = patientService;
            // _logger = logger;
            //_config = configOptions.Value;
        }

        #region  Actions
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(PatientEntity), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetPatientById(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var patient = await _patientService.GetPatientById(id);

            if (patient != null)
            {
                return Ok(patient);
            }

            return NotFound();
        }

        [HttpGet]
        [Route("Patients")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(List<PatientDTO>), (int)HttpStatusCode.OK)]
        public IActionResult GetAllPatients()
        {
            var patients = _patientService.GetAllPatients();

            if (patients.Any())
            {
                return Ok(patients);
            }

            return NotFound();
        }
        #endregion
    }
}