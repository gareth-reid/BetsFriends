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

namespace BF_API
{
    public static class BFHorseRunners
    {
        [FunctionName("BFHorseRunners")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log, ExecutionContext context)
        {
            log.LogInformation("BFHorses BetFair API accessed, Date:" + new DateTime().ToString());

            string raceId = req.Query["id"].ToString();

            List<string> runners = new List<string>();
            ICacheManager<List<string>> cacheManager = new CacheManager<List<string>>();
            var data = cacheManager.GetItem("HorseRunners");
            if (data != null)
            {
                runners = data;
            }
            else
            {
                var apiConfig = new ApiConfig(context);
                log.LogInformation("");
                runners = GetRunners(apiConfig.BetfairClient, raceId);
                cacheManager.SetItem("HorseRunners", runners, DateTimeOffset.Now.AddHours(1));
            }

            return new OkObjectResult(runners);
        }
        public static List<string> MockGetRunners()
        {
            List<string> races = new List<string>(new string[] { "R1 1740m Pace M|8/21/2020 1:12:00 PM|1.172216526", "R2 1740m Pace M| 8/21/2020 1:47:00 PM|1.172216528", "R3 1740m Pace M| 8/21/2020 2:22:00 PM|1.172216530", "R4 1740m Pace M| 8/21/2020 3:03:00 PM|1.172216532", "R5 1740m Pace M| 8/21/2020 3:42:00 PM|1.172216534", "R6 1740m Pace M| 8/21/2020 4:17:00 PM|1.172216536", "R7 1740m Pace M| 8/21/2020 4:52:00 PM|1.172216538", "R8 2270m Pace M| 8/21/2020 5:25:00 PM|1.172216540" });
            return races;
        }
        public static List<string> GetRunners(BetfairClient client, string raceId)
        {
            var marketFilter = new MarketFilter();
            marketFilter.MarketIds = new HashSet<string>() { raceId };
            marketFilter.MarketTypeCodes = new HashSet<String>() { "WIN" };

            List<MarketCatalogue> marketCatalogues = client.ListMarketCatalogue(
              marketFilter,
              BFHelpers.HorseRaceProjection(),
              MarketSort.FIRST_TO_START,
              100).Result.Response;

            var marketIds = new HashSet<String>() { raceId };
            var marketBooks = client.ListMarketBook(marketIds).Result.Response;
            List<Runner> runnersForMarket = marketBooks.First().Runners;

            var runners = marketCatalogues.First().Runners.Select(runner =>                 
                 runner.RunnerName + "|" +
                 runner.Metadata.GetValueOrDefault("JOCKEY_NAME") +
                 "^" + runner.Metadata.GetValueOrDefault("TRAINER_NAME") +
                 "^" + runner.Metadata.GetValueOrDefault("WEIGHT_VALUE") +
                 "^" + runner.Metadata.GetValueOrDefault("FORM") +
                 "^" + runner.Metadata.GetValueOrDefault("STALL_DRAW") +
                 "|" + runner.SelectionId +
                 "|" + runnersForMarket.Find(rfm => rfm.SelectionId == runner.SelectionId).LastPriceTraded
            ); ;
            //{[WEIGHT_VALUE,]}
            //{[JOCKEY_NAME, T P Mccarthy]}
            //{[TRAINER_NAME, T J Munday]}
            //{[FORM, 83]}
            //{[STALL_DRAW, 3]}
            return runners.ToList();

        }
    }
}
