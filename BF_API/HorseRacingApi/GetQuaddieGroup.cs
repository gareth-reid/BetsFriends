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
using BF_API.Data;

namespace BF_API
{
    public static class GetQuaddieGroup
    {
        [FunctionName("GetQuaddieGroup")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log, ExecutionContext context)
        {
            log.LogInformation("QuaddieBuilder API accessed, Date:" + new DateTime().ToString());

            int quaddieGroupId;
            int.TryParse(req.Query["qgId"].ToString(), out quaddieGroupId);
            List<QuaddieGroup> quaddieGroups = new List<QuaddieGroup>();

            QuaddieBuilderEngine quaddieBuilderEngine = new QuaddieBuilderEngine();
            if (quaddieGroupId == 0)
            {
                quaddieGroups = quaddieBuilderEngine.GetAll();
            }
            else
            {
                quaddieGroups.Add(quaddieBuilderEngine.GetFromApiId(quaddieGroupId));
            }
            Newtonsoft.Json.JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            };

            return new OkObjectResult(Newtonsoft.Json.JsonConvert.SerializeObject(quaddieGroups));
        }
    }
}
