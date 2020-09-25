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

namespace BF_API
{
    public static class BFSearch
    {
        [FunctionName("BFSearch")]
        public async static Task<BetfairServerResponse<List<EventResult>>> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log, ExecutionContext context)
        {

            log.LogInformation("BFSearch BetFair API accessed, Date:" + new DateTime().ToString());
            var searchString =  req.Query["searchText"].ToString();

            var marketFilter = new MarketFilter();
            //marketFilter.EventTypeIds = new HashSet<string>() { "7" };
            //marketFilter.MarketCountries = new HashSet<string>() { "AU" };
            
            marketFilter.MarketStartTime = new TimeRange()
            {
                From = DateTime.Now,
                To = DateTime.Now.AddDays(15)
            };
            marketFilter.TextQuery = searchString;
            //var events = client.ListEvents(marketFilter).Result;
            var apiConfig = new ApiConfig(context);
            log.LogInformation("");

            return await apiConfig.BetfairClient.ListEvents(
              marketFilter);
        }       
        
    }
}
