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
    public static class AddUser
    {
        [FunctionName("AddUser")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log, ExecutionContext context)
        {
            string username = req.Query["un"].ToString();
            string password = req.Query["pw"].ToString();
            string email = req.Query["em"].ToString();

            log.LogInformation("AddUser API accessed, Date:" + new DateTime().ToString());
            UserEngine userEngine = new UserEngine();
            userEngine.AddUser(username, password, email);
            return new OkObjectResult("");
        }
        
    }
}
