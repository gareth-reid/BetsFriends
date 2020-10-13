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
using BF_API.Data.Engine;
using Microsoft.EntityFrameworkCore;

namespace BF_API
{
    public static class GetMulti
    {
        private static ExecutionContext _context;
        [FunctionName("GetMulti")]
        public static List<MultiBuilder> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log, ExecutionContext context)
        {
            _context = context;
            log.LogInformation("GetMulti API accessed, Date:" + new DateTime().ToString());
            var multiId =  req.Query["id"].ToString();
            var count = req.Query["count"].ToString();
            var a = req.Query["ns"].ToString();

            bool loadNonSelected;
            Boolean.TryParse(req.Query["ns"].ToString(), out loadNonSelected);

            MultiEngine multiEngine = new MultiEngine();
            List<MultiBuilder> multis;

            if (multiId != "")
            {
                multis = new List<MultiBuilder>() { multiEngine.GetFromApiId(multiId) };
            }
            else
            {
                multis = multiEngine.GetLatest(count);
            }
            
            return multis;
        }        
    }
    
}
