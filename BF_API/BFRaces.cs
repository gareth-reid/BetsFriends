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

namespace BF_API
{
    public static class BFRaces
    {
        [FunctionName("BFRaces")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("BFHorses BetFair API accessed, Date:" + new DateTime().ToString());

            bool mock = req.Query["mock"].ToString() != "";
            string venueId = req.Query["id"].ToString();

            List<string> races = new List<string>();
            if (!mock)
            {
                BetfairClient client = new BetfairClient("UhwmsL3EqCwjEKwH");
                client.Login(@"/etc/client-2048.p12", "REDsky.123", "garethreid123@gmail.com", "REDsky.123");
                races = GetRaces(client, venueId);
            }
            else
            {
                races = MockGetRaces();
            }            

            return new OkObjectResult(races);
        }
        public static List<string> MockGetRaces()
        {
            List<string> races = new List<string>(new string[] { "R1 1100m Mdn|8/21/2020 11:03:00 AM|1.172218585", "R2 1100m Mdn|8/21/2020 11:38:00 AM|1.172218587", "R3 1100m 3yo|8/21/2020 12:17:00 PM|1.172218589", "R4 1100m 3yo|8/21/2020 12:52:00 PM|1.172218591", "R5 1400m Mdn|8/21/2020 1:27:00 PM|1.172218593", "R6 860m Hcap|8/21/2020 2:04:00 PM|1.172218595", "R7 1100m Hcap|8/21/2020 2:42:00 PM|1.172218597", "R8 1100m Hcap|8/21/2020 3:17:00 PM|1.172218599", "R9 1400m Hcap|8/21/2020 3:57:00 PM|1.172218601", "R10 1400m Hcap|8/21/2020 4:34:00 PM|1.172218603" });
            return races;
        }
        public static List<string> GetRaces(BetfairClient client, string venueId)
        {
            var marketFilter = new MarketFilter();
            marketFilter.EventIds = new HashSet<string>() { venueId };            
            marketFilter.MarketTypeCodes = new HashSet<String>() { "WIN" };
            //var events = client.ListEvents(marketFilter).Result;

            List<MarketCatalogue> marketCatalogues = client.ListMarketCatalogue(
              marketFilter,
              BFHelpers.HorseRaceProjection(),
              MarketSort.FIRST_TO_START,
              100).Result.Response;

            var races = marketCatalogues.Select(race =>
                 race.MarketName + "|" + race.Description.MarketTime.ToLocalTime() + "|" + race.MarketId
            );

            return races.ToList();        

        }
    }
}
