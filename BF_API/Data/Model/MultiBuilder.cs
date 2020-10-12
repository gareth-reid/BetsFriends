using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace BF_API.Data
{
    public class MultiBuilder
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MultiBuilderId { get; set; }
        public string MultiName { get; set; }
        public DateTime DateAdded { get; set; }
        public ICollection<SingleMarket> Markets { get; set; }
        public ICollection<SingleMarket> FinalMarkets { get; set; }
        public MultiBuilder()
        {
            FinalMarkets = new List<SingleMarket>();
        }

        public void Execute()
        {
            var events = Markets.GroupBy(market => market.EventId);
            foreach (IGrouping<String, SingleMarket> e in events)
            {
                var orderedByRationMarkets = e.Where(m => (m.BackLayRatio > 0 && m.BackLayRatio < 0.3) && m.Price < 3 && m.Price > 1.05);
                if (orderedByRationMarkets.Count() > 0)
                {
                    FinalMarkets.Add(orderedByRationMarkets.OrderBy(m => m.BackLayRatio).First());
                }                
            }
            FinalMarkets = FinalMarkets.OrderBy(m => m.BackLayRatio).ToList();
            ExecuteTotalPrice();
        }

        public double TotalPrice { get; set; }
        public void ExecuteTotalPrice()
        {
            TotalPrice = FinalMarkets.Select(fm => fm.Price).Aggregate((a, x) => a * x);
        }
    }
}
