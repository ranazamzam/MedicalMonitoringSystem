using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MedicalBookingSystem.APIGateway.Aggregator.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MedicalBookingSystem.APIGateway
{
    public class EventsController : Controller
    {
        private IEventService _eventService;
        private IPatientService _patientService;
        private IDoctorService _doctorService;
        private readonly ILogger _logger;

        public EventsController(IEventService eventService, IPatientService patientService, IDoctorService doctorService, ILogger<EventsController> logger)
        {
            _eventService = eventService;
            _patientService = patientService;
            _doctorService = doctorService;
            _logger = logger;

        }

        [HttpGet]
        [Route("Events")]
        public async Task<IActionResult> GetAllEvents()
        {
            try
            {
                var eventsList = await _eventService.GetEvents();

                // Get patient name and doctor name from doctor and patient services for each event
                if (eventsList != null && eventsList.Any())
                {
                    foreach (var @event in eventsList)
                    {
                        var patientData = await _patientService.GetByIdAsync(@event.PatientId);
                        @event.PatientName = patientData != null ? patientData.Name : string.Empty;

                        if (@event.DoctorId.HasValue)
                        {
                            var doctorData = await _doctorService.GetByIdAsync(@event.DoctorId.Value);
                            @event.DoctorName = doctorData != null ? doctorData.Name : string.Empty;
                        }
                    }

                    return Ok(eventsList);
                }

                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message);
                throw new Exception("An exception occured.");
            }
        }
    }
}