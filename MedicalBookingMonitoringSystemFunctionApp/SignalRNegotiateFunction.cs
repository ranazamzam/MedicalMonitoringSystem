using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;

namespace MedicalBookingMonitoringSystemFunctionApp
{
    public static class SignalRNegotiateFunction
    {
        /// <summary>
        /// returns Azure Signal R information (Url & AccessToken) to the caller for connecting to signalR hub
        /// </summary>
        /// <param name="req"></param>
        /// <param name="info"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName("negotiate")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req,
            [SignalRConnectionInfo(HubName = "broadcastConflicts")]SignalRConnectionInfo info,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            return info != null
                ? (ActionResult)new OkObjectResult(info)
                : new NotFoundObjectResult("Failed to load SignalR Info.");
            
        }
    }
}
