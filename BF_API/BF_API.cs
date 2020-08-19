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
    public static class BF_API
    {
        [FunctionName("BF_API")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            
            string name = req.Query["name"];
            
            BetfairClient client = new BetfairClient("UhwmsL3EqCwjEKwH");
            client.Login(@"/etc/client-2048.p12", "REDsky.123", "garethreid123@gmail.com", "REDsky.123");


            Test(client);
            /*string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;
            */
            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";
            
            return new OkObjectResult(responseMessage);
        }


        public static void Test(BetfairClient client)
        {
            var marketFilter = new MarketFilter();
            marketFilter.EventTypeIds = new HashSet<string>() { "7" };
            marketFilter.MarketCountries = new HashSet<string>() { "AU" };
            marketFilter.MarketStartTime = new TimeRange()
            {
                From = DateTime.Now,
                To = DateTime.Now.AddDays(2)
            };
            marketFilter.MarketTypeCodes = new HashSet<String>() { "WIN" };
            var events = client.ListEvents(marketFilter).Result;

            List<MarketCatalogue> marketCatalogues = client.ListMarketCatalogue(
              marketFilter,
              BFHelpers.HorseRaceProjection(),
              MarketSort.FIRST_TO_START,
              100).Result.Response;

            var venues = marketCatalogues.Select(mark =>
               mark.Event.Venue).Distinct();
            foreach (MarketCatalogue mc in marketCatalogues)
            {
                Console.WriteLine(mc.Event.Venue);
                Console.WriteLine(mc.MarketName);

                var marketIds = new HashSet<String>() { mc.MarketId };
                var marketBooks = client.ListMarketBook(marketIds).Result.Response;
                List<Runner> runnersForMarket = marketBooks.First().Runners;
                Console.WriteLine("************RUNNERS**************");
                foreach (Runner r in runnersForMarket)
                {
                    var rc = mc.Runners.Find(runner => runner.SelectionId.Equals(r.SelectionId));
                    Console.WriteLine("Horse: " + rc.RunnerName +
                        " | Last Price:" + r.LastPriceTraded +
                        " | Status: " + r.Status);
                }
            }

        }
    }
}
