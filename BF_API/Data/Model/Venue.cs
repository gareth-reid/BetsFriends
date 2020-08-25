using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;


namespace BF_API.Data
{
    public class Venue
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int VenueId { get; set; }
        public int BfEventId { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public DateTime? OpenDate { get; set; }
        public DateTime? DateModified { get; set; }

        public ISet<Race> Races { get; set; }
    }
}
