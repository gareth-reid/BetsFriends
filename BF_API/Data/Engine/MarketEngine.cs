using System;
using System.Collections.Generic;
using System.Linq;
using BetfairNG.Data;
using Microsoft.EntityFrameworkCore;

namespace BF_API.Data.Engine
{
    public class MarketEngine : IEngine<SingleMarket>
    {   
        public SingleMarket Clone(SingleMarket newMulti, SingleMarket existingMulti)
        {            
            return existingMulti;
        }

        public SingleMarket GetFromApiId(object id)
        {
            int marketId;
            int.TryParse(id.ToString(), out marketId);
            using (var db = new DataContext())
            {
                var markets = db.Markets
                    .Where(r => r.Id == marketId)                    
                    .FirstOrDefault();
                return markets;
            }
        }

        public List<SingleMarket> GeAllForEvent(string eventId)
        {          
            using (var db = new DataContext())
            {
                var markets = db.Markets
                    .Where(r => r.EventId == eventId)
                    .ToList();
                return markets;
            }
        }
    }
}

