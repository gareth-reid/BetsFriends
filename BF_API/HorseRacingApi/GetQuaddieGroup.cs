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
    public static class QuaddieBuilder
    {
        [FunctionName("QuaddieBuilder")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log, ExecutionContext context)
        {            
            log.LogInformation("QuaddieBuilder API accessed, Date:" + new DateTime().ToString());

            string quaddieGroupId = req.Query["qgId"].ToString();
            string runnerId = req.Query["runnerId"].ToString();
            string user = req.Query["user"].ToString();
            string description = req.Query["description"].ToString();
            QuaddieBuilderEngine quaddieBuilderEngine = new QuaddieBuilderEngine();
            var success = quaddieBuilderEngine.Execute(runnerId, quaddieGroupId, user);
            return new OkObjectResult(success);
        }        
    }
}
