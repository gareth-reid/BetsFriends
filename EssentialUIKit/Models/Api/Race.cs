using System;
using System.Collections.Generic;


namespace EssentialUIKit.Models.Api
{
    public class Race
    {
        public int RaceId { get; set; }
        public string BfMarketId { get; set; }
        public string Description { get; set; }
        public DateTime RaceTime { get; set; }
        public DateTime DateModified { get; set; }        
        public Venue Venue { get; set; }
        public ISet<Runner> RaceRunners { get; set; }

    }
}
