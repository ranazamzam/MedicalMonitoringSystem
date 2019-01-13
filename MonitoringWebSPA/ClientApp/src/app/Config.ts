export let Config = {
  Urls: {
    //AzureSignalrUrl: 'http://localhost:7071/api/',
    //APIGateway: 'http://localhost:19081/MedicalBookingSystem/MedicalBookingSystem.APIGateway/',
    //APIGatewaySignalR: 'http://localhost:19081/MedicalBookingSystem/MedicalBookingSystem.APIGateway/eventsGenerator',
    //APIGatewayEvents: 'http://localhost:19081/MedicalBookingSystem/MedicalBookingSystem.APIGateway/Events'
    AzureSignalrUrl: 'https://medicalbookingmonitoringsystemfunctionapp.azurewebsites.net/api/',
    APIGateway: 'http://medicalsystemmonitoring.northeurope.cloudapp.azure.com:8081',
    APIGatewaySignalR: 'http://medicalsystemmonitoring.northeurope.cloudapp.azure.com:19081/MedicalBookingSystem/MedicalBookingSystem.APIGateway/eventsGenerator',
    //APIGatewaySignalR: 'http://medicalsystemmonitoring.northeurope.cloudapp.azure.com:8081/eventsGenerator',
    APIGatewayEvents: 'http://medicalsystemmonitoring.northeurope.cloudapp.azure.com:8081/Events'
  }
};
