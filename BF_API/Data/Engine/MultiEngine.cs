using System;
using System.Collections.Generic;
using System.Linq;
using BetfairNG.Data;
using Microsoft.EntityFrameworkCore;

namespace BF_API.Data.Engine
{
    public class MultiEngine : IEngine<MultiBuilder>
    {   
        public MultiBuilder Clone(MultiBuilder newMulti, MultiBuilder existingMulti)
        {            
            return existingMulti;
        }

        public MultiBuilder GetFromApiId(object id)
        {
            int multiId;
            int.TryParse(id.ToString(), out multiId);
            using (var db = new DataContext())
            {
                var multis = db.MultiBuilders
                    .Include("FinalMarkets")
                    .Include("Markets")
                    .Where(r => r.MultiBuilderId == multiId)                    
                    .FirstOrDefault();
                return multis;
            }
        }

        public List<MultiBuilder> GetLatest(string amount = "1")
        {
            int amountToTake;
            int.TryParse(amount, out amountToTake);
            if (amountToTake == 0)
            {
                amountToTake = 1;
            }
            using (var db = new DataContext())
            {
                var multis = db.MultiBuilders
                    .Include("FinalMarkets")
                    .Include("Markets")
                    .OrderByDescending(m => m.DateAdded)
                    .Take(amountToTake)
                    .ToList();

                return multis;
            }
        }

        public List<MultiBuilder> GetRecent()
        {
            using (var db = new DataContext())
            {
                var multis = db.MultiBuilders
                    .Include("FinalMarkets")
                    .Include("Markets")
                    .Where(m => m.DateAdded > DateTime.Now.AddDays(-10))                    
                    .ToList();

                return multis;
            }
        }
    }
}

