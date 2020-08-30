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
using System.Runtime.Caching;
using BF_API.CacheManager;
using BF_API.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;


namespace BF_API
{
    public static class UpdateDatabase
    {
        [FunctionName("UpdateDatabase")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log, ExecutionContext context)
        {
            try
            {
                using (var db = new DataContext())
                {
                    //db.Database.EnsureDeleted();
                    db.Database.EnsureCreated();
                }
                //BFHorseVenues.Run(req, log, context);
                return new OkObjectResult(new List<string>());
            } catch (Exception e)
            {
                log.LogInformation("Error in update database, error (log): " + e.Message.ToString() + " @" + new DateTime().ToString());
                throw new Exception("Error in update database, error (exception): " + e.Message.ToString() + " @" + new DateTime().ToString());
            }
        }
    }
}
