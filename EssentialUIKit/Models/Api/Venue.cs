using System;
using System.Collections.Generic;

namespace EssentialUIKit.Models.Api
{
    public class Venue
    {
        public int VenueId { get; set; }
        public int BfEventId { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public DateTime? OpenDate { get; set; }
        public DateTime? DateModified { get; set; }

        public ISet<Race> Races { get; set; }
    }
}
