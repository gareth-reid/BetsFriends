using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using BetfairNG;
using BetfairNG.Data;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using BF_API.CacheManager;
using BF_API.Data.Engine;

namespace BF_API
{
    public static class CreateQuaddieGroup
    {
        [FunctionName("CreateQuaddieGroup")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log, ExecutionContext context)
        {            
            log.LogInformation("QuaddieBuilder API accessed, Date:" + new DateTime().ToString());

            string quaddieGroupId = req.Query["qgId"].ToString();
            string user = req.Query["user"].ToString();
            string description = req.Query["desc"].ToString();
            string venueApiId = req.Query["vId"].ToString();
            QuaddieBuilderEngine quaddieBuilderEngine = new QuaddieBuilderEngine();
            var quaddieGroup = quaddieBuilderEngine.Create(description, quaddieGroupId, venueApiId, user);
            return new OkObjectResult(quaddieGroup.QuaddieGroupId);
        }        
    }
}
