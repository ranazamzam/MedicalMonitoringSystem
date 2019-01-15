using EventBus.GenericEventBus.Interfaces;
using MedicalBookingSystem.APIGateway.Aggregator.Interfaces;
using MedicalBookingSystem.APIGateway.Aggregator.SignalRHub;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MedicalBookingSystem.APIGateway
{
    public class MedicalBookingSystemGeneratedEventIntegrationEventHandler : IIntegrationEventHandler<MedicalBookingSystemGeneratedEventIntegrationEvent>
    {
        #region Properties
        private INotificationService _notificationService;
        private IPatientService _patientService;
        private IDoctorService _doctorService;
        #endregion

        public MedicalBookingSystemGeneratedEventIntegrationEventHandler(INotificationService notificationService, IPatientService patientService, IDoctorService doctorService)
        {
            _notificationService = notificationService;
            _patientService = patientService;
            _doctorService = doctorService;
        }

        /// <summary>
        /// Handles the event published from azure service bus which contains a message for the new generated event
        /// As an API aggregator, it will get the patient name and doctor name from the patient and doctor microservices
        /// and notify all clients that a new event is generated using signalR
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        public async Task Handle(MedicalBookingSystemGeneratedEventIntegrationEvent @event)
        {
            // Get patient name and doctor name from doctor and patient services
            var patientData = await _patientService.GetByIdAsync(@event.PatientId);
            @event.PatientName = patientData != null ? patientData.Name : string.Empty;

            if (@event.DoctorId.HasValue)
            {
                var doctorData = await _doctorService.GetByIdAsync(@event.DoctorId.Value);
                @event.DoctorName = doctorData != null ? doctorData.Name : string.Empty;
            }

            // Notify connected clients of the new events
            await _notificationService.BroadCastGeneratedEvent(@event);
        }
    }
}
