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
    public static class BFHorseVenues
    {
        [FunctionName("BFHorseVenues")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log, ExecutionContext context)
        {

            log.LogInformation("BFHorseVenues BetFair API accessed, Date:" + new DateTime().ToString());

            List<string> venues = new List<string>();

            ICacheManager<List<string>> cacheManager = new CacheManager<List<string>>();
            var data = cacheManager.GetItem("HorseVenues");
            if (data != null)
            {
                venues = data;
            }
            else
            {
                var apiConfig = new ApiConfig(context);
                log.LogInformation("");
                venues = GetVenues(apiConfig.BetfairClient, log);
                cacheManager.SetItem("HorseVenues", venues, DateTimeOffset.Now.AddHours(3));
            }

            string responseMessage = $"Hello, {venues}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(venues);
        }
        public static List<string> MockGetVenues()
        {
            List<string> venues = new List<string>(new string[] { "Penrith | Penr(AUS) 20th Aug | 29964927", "Terang | Tera(AUS) 20th Aug | 29964957", "Gatton | Gatt(AUS) 21st Aug | 29966934", "Canberra | Canb(AUS) 21st Aug | 29966666", "Bendigo | Bend(AUS) 21st Aug | 29966930", "Taree | Tare(AUS) 21st Aug | 29966665", "Wagga | Wagg(AUS) 21st Aug | 29966659", "Melton | Melt(AUS) 21st Aug | 29966657", "Carnarvon | Carn(AUS) 21st Aug | 29966653", "Newcastle | Newc(AUS) 21st Aug | 29966660", "Launceston | Laun(AUS) 21st Aug | 29967323", "Albion Park | APrk(AUS) 21st Aug | 29966684", "Mildura | Mdra(AUS) 21st Aug | 29966661", "Gloucester Park | GlPk(AUS) 21st Aug | 29966683" });
            return venues;
        }
        public static List<string> GetVenues(BetfairClient client, ILogger log)
        {
            var marketFilter = new MarketFilter();
            marketFilter.EventTypeIds = new HashSet<string>() { "7" };
            marketFilter.MarketCountries = new HashSet<string>() { "AU" };
            marketFilter.MarketStartTime = new TimeRange()
            {
                From = DateTime.Now,
                To = DateTime.Now.AddDays(3)
            };
            marketFilter.MarketTypeCodes = new HashSet<String>() { "WIN" };
            //var events = client.ListEvents(marketFilter).Result;

            List<EventResult> events = client.ListEvents(
              marketFilter).Result.Response;
            VenueEngine venueEngine = new VenueEngine();
            venueEngine.Execute(events.Select(e =>
               e.Event).Distinct().ToList(), log);
            var venues = events.Select(mark =>
                mark.Event.Venue + "|" + mark.Event.Name + "|" + mark.Event.Id).Distinct();


            return venues.ToList();
        }
    }
}
