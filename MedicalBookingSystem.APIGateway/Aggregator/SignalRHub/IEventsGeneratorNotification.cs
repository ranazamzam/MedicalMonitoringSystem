using System.Threading.Tasks;

namespace MedicalBookingSystem.APIGateway.Aggregator.SignalRHub
{
    public interface IEventsGeneratorNotification
    {
        Task ReceiveNewEvent(MedicalBookingSystemGeneratedEventIntegrationEvent message);
    }
}
