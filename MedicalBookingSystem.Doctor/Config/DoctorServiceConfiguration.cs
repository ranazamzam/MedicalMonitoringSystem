using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalBookingSystem.Doctor.Config
{
    public class DoctorServiceConfiguration
    {
        public static string DefaultConnectiontring;

        static DoctorServiceConfiguration()
        {
            var codeContext = FabricRuntime.GetActivationContext();

            if (codeContext == null)
            {
                // sanity check
                throw new ApplicationException("CodePackageActivationContext is null");
            }

            ConfigurationPackage configurationPackage = codeContext.GetConfigurationPackageObject("Config");

            if (configurationPackage.Settings?.Sections == null || !configurationPackage.Settings.Sections.Contains("ConnectionStrings"))
            {
                return;
            }

            var param = configurationPackage.Settings.Sections["ConnectionStrings"].Parameters;

            if (param.Contains("DefaultConnection"))
            {
                DefaultConnectiontring = param["DefaultConnection"].Value;
            }
        }
    }
}
