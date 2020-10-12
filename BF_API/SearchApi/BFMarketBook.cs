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
    public static class BFMarketBook
    {
        [FunctionName("BFMarketBook")]
        public async static Task<BetfairServerResponse<List<MarketBook>>> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log, ExecutionContext context)
        {

            log.LogInformation("BFMarketBook BetFair API accessed, Date:" + new DateTime().ToString());
            var marketId =  req.Query["marketId"].ToString();
                        
            var marketIds = new List<string>() { marketId };
                       
            var apiConfig = new ApiConfig(context);
            log.LogInformation("");

            return await apiConfig.BetfairClient.ListMarketBook(
              marketIds, new PriceProjection() { Virtualise = true, PriceData = new HashSet<PriceData>() { PriceData.SP_AVAILABLE, PriceData.SP_TRADED, PriceData.EX_TRADED, PriceData.EX_BEST_OFFERS, PriceData.EX_ALL_OFFERS } }, null);
        }       
        
    }
}
