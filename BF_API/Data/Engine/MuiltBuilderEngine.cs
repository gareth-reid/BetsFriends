using System;
using System.Collections.Generic;
using System.Linq;
using BetfairNG.Data;
using BF_API.Entities;
using BF_API.Helpers.ExtensionMethods;
using Microsoft.EntityFrameworkCore;

namespace BF_API.Data.Engine
{
    public class MuiltBuilderEngine
    {
        private ApiConfig _apiConfig;
        public MuiltBuilderEngine(ApiConfig apiConfig)
        {
            _apiConfig = apiConfig;
        }

        public MultiBuilder Execute(CustomMarketFilter filter, bool addToMulti)
        {            
            MultiBuilder multiBuilder = new MultiBuilder();            

            var marketCatalogueFilter = new MarketFilter();
            if (filter.EventTypes != null && filter.EventTypes.Count > 0)
            {
                marketCatalogueFilter.EventTypeIds = filter.EventTypes;
            }
            if (filter.MarketTypes != null && filter.MarketTypes.Count > 0)
            {
                marketCatalogueFilter.MarketTypeCodes = filter.MarketTypes;
            }
            if (filter.EventIds != null && filter.EventIds.Count > 0)
            {
                marketCatalogueFilter.EventIds = filter.EventIds;
            }            

            var timeRange = new TimeRange();
            bool hasTimeFilter = false;
            if (filter.DateFrom.HasValue)
            {
                hasTimeFilter = true;
                timeRange.From = filter.DateFrom.Value;
            }
            if (filter.DateTo.HasValue)
            {
                hasTimeFilter = true;
                timeRange.To = filter.DateTo.Value;
            }
            if (hasTimeFilter)
            {
                marketCatalogueFilter.MarketStartTime = timeRange;
            }

            List<MarketCatalogue> marketCatalogues = _apiConfig.BetfairClient.ListMarketCatalogue(
              marketCatalogueFilter, new HashSet<MarketProjection>()
              { MarketProjection.EVENT, MarketProjection.EVENT_TYPE, MarketProjection.COMPETITION, MarketProjection.RUNNER_DESCRIPTION },
              null, filter.Count).Result.Response;

            List<SingleMarket> marketList = new List<SingleMarket>();
            foreach (MarketCatalogue marketCatalogue in marketCatalogues)
            {
                List<MarketBook> marketBook = _apiConfig.BetfairClient.ListMarketBook(
                    new HashSet<string>() { marketCatalogue.MarketId.ToString() },
                    new PriceProjection() { Virtualise = true, PriceData = new HashSet<PriceData>() { PriceData.SP_AVAILABLE, PriceData.SP_TRADED, PriceData.EX_TRADED, PriceData.EX_BEST_OFFERS, PriceData.EX_ALL_OFFERS } })
                    .Result.Response;

                if (marketBook != null && marketBook.Count > 0)
                {
                    int order = 0;
                    foreach (BetfairNG.Data.Runner runner in marketBook.First().Runners.Take(marketBook.First().Runners.Count > 1 ? 5 : 1))
                    {
                        SingleMarket market = new SingleMarket();
                        market.EventName = marketCatalogue.Event.IsNull().Name;
                        market.MarketName = marketCatalogue.MarketName;
                        market.EventId = marketCatalogue.Event.Id;
                        market.MarketId = marketCatalogue.MarketId;
                        market.EventType = marketCatalogue.EventType.IsNull().Name;
                        market.Competition = marketCatalogue.Competition.IsNull().Name;
                        market.Date = marketCatalogue.Event.IsNull().OpenDate;
                        double layPrice = 0, backPrice = 0, avlToBack = 0, avlToLay = 0;
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
                            layPrice = runner.ExchangePrices.AvailableToLay.First().Price;
                            avlToLay = runner.ExchangePrices.AvailableToLay.First().Size;
                        }

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
                        market.LayPrice = layPrice;
                        market.BackPrice = backPrice;
                        market.AvlToBack = avlToBack;
                        market.AvlToLay = avlToLay;
                        market.DateAdded = DateTime.Now;
                        order++;

                        if (addToMulti) //less money available to back (more lay) so more people backing = good bet
                        {
                            if (backPrice > 0 && avlToBack > 0 && avlToBack < avlToLay)
                            {
                                marketList.Add(market);
                            }
                        }
                        else
                        {
                            marketList.Add(market);                            
                        }
                    }
                }
            }

            if (addToMulti)
            {
                multiBuilder.Markets = marketList;
                multiBuilder.MultiName = DateTime.Now.ToString();
                multiBuilder.DateAdded = DateTime.Now;

                //log.LogInformation("");            
                using (var db = new DataContext())
                {
                    db.Entry(multiBuilder).State = EntityState.Modified;
                    db.MultiBuilders.Add(multiBuilder);
                    db.SaveChanges();
                }
            }
            else
            {
                using (var db = new DataContext())
                {
                    multiBuilder.Markets = marketList;//for return TODO refactor
                    db.Markets.AddRange(marketList);
                    db.SaveChanges();
                }
            }

            return multiBuilder;
        }
    }
}
