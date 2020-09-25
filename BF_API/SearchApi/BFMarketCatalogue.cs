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
    public static class BFMarketCatalogue
    {
        [FunctionName("BFMarketCatalogue")]
        public async static Task<BetfairServerResponse<List<MarketCatalogue>>> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log, ExecutionContext context)
        {

            log.LogInformation("BFMarketCatalogue BetFair API accessed, Date:" + new DateTime().ToString());
            var eventId =  req.Query["eventId"].ToString();
            var marketFilter = new MarketFilter();
            marketFilter.EventIds = new HashSet<string>() { eventId };
                                   
            var apiConfig = new ApiConfig(context);
            log.LogInformation("");

            return await apiConfig.BetfairClient.ListMarketCatalogue(
              marketFilter, null, null, 100);
        }       
        
    }
}
