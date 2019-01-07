using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalBookingSystem.APIGateway.Aggregator.Config
{
    public static class APIGatewayConfiguration
    {
        public static string ReverseProxyUri;
        public static string EventsAzureFunctionUrl;

        static APIGatewayConfiguration()
        {
            var codeContext = FabricRuntime.GetActivationContext();

            if (codeContext == null)
            {
                // sanity check
                throw new ApplicationException("CodePackageActivationContext is null");
            }

            ConfigurationPackage configurationPackage = codeContext.GetConfigurationPackageObject("Config");

            if (configurationPackage.Settings?.Sections == null || !configurationPackage.Settings.Sections.Contains("ServicesReverseProxy"))
            {
                return;
            }

            var param = configurationPackage.Settings.Sections["ServicesReverseProxy"].Parameters;

            if (param.Contains("ReverseProxyUri"))
            {
                ReverseProxyUri = param["ReverseProxyUri"].Value;
            }

            if (param.Contains("EventsAzureFunction"))
            {
                EventsAzureFunctionUrl = param["EventsAzureFunction"].Value;
            }
        }
    }
}
