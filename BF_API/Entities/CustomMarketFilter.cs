using System;
using System.Collections.Generic;

namespace BF_API.Entities
{
    public class CustomMarketFilter
    {
        public HashSet<string> EventTypes;
        public HashSet<string> MarketTypes;
        public HashSet<string> EventIds;
        public int Count;
        public DateTime? DateFrom;
        public DateTime? DateTo;

        public CustomMarketFilter()
        {
        }
    }
}
