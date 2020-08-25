using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;


namespace BF_API.Data
{
    public class Race
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RaceId { get; set; }
        public string BfMarketId { get; set; }
        public string Description { get; set; }
        public DateTime RaceTime { get; set; }
        public DateTime DateModified { get; set; }        
        public Venue Venue { get; set; }
        public ISet<Runner> RaceRunners { get; set; }

    }
}
