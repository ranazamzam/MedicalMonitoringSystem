using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
        [ProducesResponseType(typeof(Patient), (int)HttpStatusCode.OK)]
        public IActionResult GetItemById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest();
            }

            var patient =  _patientService.GetPatientById(id);
            
            if (patient != null)
            {
                return Ok(patient);
            }

            return NotFound();
        }
        #endregion
    }
}