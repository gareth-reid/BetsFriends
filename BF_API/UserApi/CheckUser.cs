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
    public static class CheckUser
    {
        [FunctionName("CheckUser")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log, ExecutionContext context)
        {
            string username = req.Query["un"].ToString(); //email
            string password = req.Query["pw"].ToString();

            log.LogInformation("AddUser API accessed, Date:" + new DateTime().ToString());
            UserEngine userEngine = new UserEngine();
            string displayName = userEngine.GetUser(username, password);
            return new OkObjectResult(displayName.ToString());
        }
        
    }
}
