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

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using BF_API.Data.Engine;
using BF_API.Data;
using BF_API.Helpers.ExtensionMethods;
using BF_API.Entities;

namespace BF_API
{
    public static class BFBuildMulti
    {
        [FunctionName("BFBuildMulti")]
        public static MultiBuilder Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log, ExecutionContext context)
        {

            log.LogInformation("BFBuildMulti BetFair API accessed, Date:" + new DateTime().ToString());            
            ICacheManager<MultiBuilder> cacheManager = new CacheManager<MultiBuilder>();
            var apiConfig = new ApiConfig(context);
            var muiltBuilderEngine = new MuiltBuilderEngine(apiConfig);
            var marketfilter = new CustomMarketFilter()
            {
                EventTypes = new HashSet<string> { "1" },
                MarketTypes = new HashSet<string> { "MATCH_ODDS", "OVER_UNDER_25", "OVER_UNDER_15" },
                Count = 20,
                DateFrom = DateTime.Now.AddHours(.5),
                DateTo = DateTime.Now.AddHours(10)

            };

            var multi = muiltBuilderEngine.Execute(marketfilter, true);
            multi.Execute();
            return multi;
        }
    }
}
