using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Doctor.Domain.DataTransferObjects;
using Doctor.Domain.Models;
using Doctor.Services.Interfaces;


namespace MedicalBookingSystem.Doctor.Controllers
{
    [Route("api/[controller]")]
    public class DoctorController : Controller
    {
        private readonly IDoctorService _doctorService;
        private readonly ILogger _logger;

        public DoctorController(IDoctorService doctorService, ILogger<DoctorController> logger)
        {
            _doctorService = doctorService;
            _logger = logger;
        }

        #region  Actions
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(DoctorEntity), (int)HttpStatusCode.OK)]
        public IActionResult GetDocotorById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest();
                }

                var doctor = _doctorService.GetDoctorById(id);

                if (doctor != null)
                {
                    return Ok(doctor);
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
        [Route("Doctors")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(List<DoctorDTO>), (int)HttpStatusCode.OK)]
        public IActionResult GetAllDoctors()
        {
            try
            {
                var doctors = _doctorService.GetAllDoctors();

                if (doctors.Any())
                {
                    return Ok(doctors);
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