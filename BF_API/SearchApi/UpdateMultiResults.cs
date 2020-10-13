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
    public static class UpdateMultiResults
    {
        private static ExecutionContext _context;
        [FunctionName("UpdateMultiResults")]
        public static string Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log, ExecutionContext context)
        {
            _context = context;
            log.LogInformation("UpdateMultiResults API accessed, Date:" + new DateTime().ToString());
            var multiId = req.Query["id"].ToString();
            MultiEngine multiEngine = new MultiEngine();
            List<MultiBuilder> multis;
            
            if (multiId == "")
            {
                multis = multiEngine.GetRecent();
            }
            else
            {
                multis = new List<MultiBuilder>() { multiEngine.GetFromApiId(multiId) };
            }

            foreach (MultiBuilder multi in multis)
            {                
                UpdateResults(multi.Markets);                
                UpdateResults(multi.FinalMarkets);
            }

            return "Success";
        }

        public static void UpdateResults(ICollection<SingleMarket> markets)
        {
            foreach (SingleMarket sm in markets)
            {
                if ((sm.Status == "OPEN" || sm.Status == "ACTIVE") && sm.Date.HasValue && sm.Date.Value < DateTime.Now)
                {
                    //GET RESULTS                
                    var apiConfig = new ApiConfig(_context);
                    IEnumerable<MarketBook> marketBooks = apiConfig.BetfairClient.ListMarketBook(
                     new HashSet<String>() { sm.MarketId }).Result.Response;

                    var marketBook = marketBooks.First();
                    sm.Status = marketBook.Runners.Find(
                        runner => runner.SelectionId == sm.SelectionId).Status.ToString();
                    using (var db = new DataContext())
                    {
                        db.Markets.Add(sm);
                        db.Entry(sm).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
        }
    }
    
}
