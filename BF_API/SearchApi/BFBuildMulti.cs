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
            MultiBuilder multiBuilder = new MultiBuilder();
            var data = cacheManager.GetItem("BuildMulti");
            if (data != null)
            {
                multiBuilder = data;
            }
            else
            {
                var apiConfig = new ApiConfig(context);
                var marketCatalogueFilter = new MarketFilter();
                marketCatalogueFilter.EventTypeIds = new HashSet<string>() { "1" };
                marketCatalogueFilter.MarketTypeCodes = new HashSet<String>() { "MATCH_ODDS", "OVER_UNDER_25", "OVER_UNDER_15" };//, "OVER_UNDER_25" };
                //marketCatalogueFilter.EventIds = new HashSet<string>() { "30030177" };
                marketCatalogueFilter.MarketStartTime = new TimeRange()
                {
                    From = DateTime.Now.AddHours(4),
                    To = DateTime.Now.AddHours(100)
                };
                List<MarketCatalogue> marketCatalogues = apiConfig.BetfairClient.ListMarketCatalogue(
                  marketCatalogueFilter, new HashSet<MarketProjection>()
                  { MarketProjection.EVENT, MarketProjection.EVENT_TYPE, MarketProjection.COMPETITION, MarketProjection.RUNNER_DESCRIPTION },
                  null, 100).Result.Response;

                List<SingleMarket> marketList = new List<SingleMarket>();
                foreach (MarketCatalogue marketCatalogue in marketCatalogues)
                {
                    SingleMarket market = new SingleMarket();
                    market.EventName = marketCatalogue.Event.IsNull().Name;
                    market.MarketName = marketCatalogue.MarketName;
                    market.EventId = marketCatalogue.Event.Id;
                    market.MarketId = marketCatalogue.MarketId;
                    market.EventType = marketCatalogue.EventType.IsNull().Name;
                    market.Competition = marketCatalogue.Competition.IsNull().Name;
                    market.Date = marketCatalogue.Event.IsNull().OpenDate;

                    List<MarketBook> marketBook = apiConfig.BetfairClient.ListMarketBook(
                        new HashSet<string>() { marketCatalogue.MarketId.ToString() },
                        new PriceProjection() { Virtualise = true, PriceData = new HashSet<PriceData>() { PriceData.SP_AVAILABLE, PriceData.SP_TRADED, PriceData.EX_TRADED, PriceData.EX_BEST_OFFERS, PriceData.EX_ALL_OFFERS } })
                        .Result.Response;

                    if (marketBook != null && marketBook.Count > 0)
                    {
                        int order = 0;
                        foreach (BetfairNG.Data.Runner runner in marketBook.First().Runners.Take(marketBook.First().Runners.Count > 1 ? 5 : 1))
                        {
                            double backPrice = 0, avlToBack = 0, avlToLay = 0;
                            if (runner.ExchangePrices.AvailableToBack.Count > 0)
                            {
                                backPrice = runner.ExchangePrices.AvailableToBack.First().Price;
                                avlToBack = runner.ExchangePrices.AvailableToBack.First().Size;

                                //not available until I get a paid apikey (non-delayed)
                                var volumePrice = "";
                                if (runner.ExchangePrices.TradedVolume.Count > 0)
                                {
                                    volumePrice = runner.ExchangePrices.TradedVolume.First().Price.ToString();
                                }
                            }
                            if (runner.ExchangePrices.AvailableToLay.Count > 0)
                            {
                                avlToLay = runner.ExchangePrices.AvailableToLay.First().Size;
                            }

                            if (backPrice > 0 && avlToBack > 0 && avlToBack < avlToLay) //less money available to back (more lay) so more people backing = good bet
                            {
                                if (runner.LastPriceTraded != null)
                                {
                                    market.LastPriceTraded = runner.LastPriceTraded;
                                }
                                market.SelectionId = runner.SelectionId;
                                market.Handicap = runner.Handicap;
                                market.BackLayRatio = avlToBack == 0 ? 0 : (avlToBack / avlToLay);
                                market.Price = backPrice;
                                market.Order = order;
                                market.Status = "OPEN";
                                marketList.Add(market);
                            }
                            order++;
                        }
                    }
                }
                
                multiBuilder.Markets = marketList;
                multiBuilder.MultiName = DateTime.Now.ToString();
                multiBuilder.DateAdded = DateTime.Now;
                multiBuilder.Execute();
                
                cacheManager.SetItem("BuildMulti", multiBuilder, DateTimeOffset.Now.AddHours(3));
            }

            //log.LogInformation("");
            using (var db = new DataContext())
            {
                db.MultiBuilders.Add(multiBuilder);
                db.SaveChanges();
            }

            return multiBuilder;
        }       
        
    }
}
