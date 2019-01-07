using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Patient.Domain.DataTransferObjects;
using Patient.Domain.Models;
using Patient.Services.Interfaces;


namespace MedicalBookingSystem.Patient.Controllers
{
    [Route("api/[controller]")]
    public class PatientController : Controller
    {
        private readonly IPatientService _patientService;
        private readonly ILogger _logger;

        public PatientController(IPatientService patientService, ILogger<PatientController> logger)
        {
            _patientService = patientService;
            _logger = logger;
        }

        #region  Actions
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(PatientEntity), (int)HttpStatusCode.OK)]
        public IActionResult GetPatientById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest();
                }

                var patient = _patientService.GetPatientById(id);

                if (patient != null)
                {
                    return Ok(patient);
                }

                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message);
                throw new Exception("An exception occured.");
            }
        }

        [HttpGet]
        [Route("Patients")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(List<PatientDTO>), (int)HttpStatusCode.OK)]
        public IActionResult GetAllPatients()
        {
            try
            {
                var patients = _patientService.GetAllPatients();

                if (patients.Any())
                {
                    return Ok(patients);
                }

                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message);
                throw new Exception("An exception occured.");
            }
        }
        #endregion
    }
}