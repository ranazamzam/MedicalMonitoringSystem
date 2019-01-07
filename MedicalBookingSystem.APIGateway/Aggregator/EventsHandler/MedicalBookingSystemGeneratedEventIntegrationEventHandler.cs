using EventBus.GenericEventBus.Interfaces;
using MedicalBookingSystem.APIGateway.Aggregator.Interfaces;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using SignalR;
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
        private INotificationService _notificationService;
        private IPatientService _patientService;
        private IDoctorService _doctorService;
        
        public MedicalBookingSystemGeneratedEventIntegrationEventHandler(INotificationService notificationService, IPatientService patientService, IDoctorService doctorService)
        {
            _notificationService = notificationService;
            _patientService = patientService;
            _doctorService = doctorService;
        }

        public async Task Handle(MedicalBookingSystemGeneratedEventIntegrationEvent @event)
        {
            // Get patient name and doctor name from doctor and patient services
            var patientData = await _patientService.GetByIdAsync(@event.PatientId);
            @event.PatientName = patientData != null ? patientData.Name : string.Empty;

            if (@event.DoctorId.HasValue) {
                var doctorData = await _doctorService.GetByIdAsync(@event.DoctorId.Value);
                @event.DoctorName = doctorData != null ? doctorData.Name : string.Empty;
            }

            // Notify connected clients of the new events
            await _notificationService.BroadCastGeneratedEvent(@event);
        }
    }
}
