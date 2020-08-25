using System;
using System.Collections.Generic;
using System.Linq;
using BetfairNG.Data;
using Microsoft.Extensions.Logging;

namespace BF_API.Data.Engine
{
    public class VenueEngine : IEngine<Venue>
    {
        public bool Execute(List<Event> events, ILogger log)
        {
            try
            {
                using (var db = new DataContext())
                {
                    foreach (Event e in events)
                    {
                        int eventId;
                        int.TryParse(e.Id, out eventId);
                        var venue = new Venue
                        {
                            Name = e.Venue,
                            Description = e.Name,
                            BfEventId = eventId,
                            OpenDate = e.OpenDate.Value.ToLocalTime(),
                            DateModified = DateTime.Now
                        };
                        var existingVenue = db.Venues
                            .Where(v => v.BfEventId == eventId)
                           .FirstOrDefault();
                        if (existingVenue != null)
                        {
                            venue = Clone(venue, existingVenue);
                        }
                        else
                        {
                            db.Venues.Add(venue);
                        }
                        db.SaveChanges();
                    }
                }
                return true;
            } catch (Exception e)
            {
                log.LogInformation("Error in save Venue, error: " + e.InnerException + " @" + new DateTime().ToString());
                return false;
            }
        }

        public Venue Clone(Venue newVenue, Venue existingVenue)
        {
            existingVenue.Name = newVenue.Name;
            existingVenue.Description = newVenue.Description;
            existingVenue.OpenDate = newVenue.OpenDate;
            existingVenue.DateModified = newVenue.DateModified;
            
            return existingVenue;
        }

        public Venue GetFromApiId(object apiId)
        {
            int eventId;
            int.TryParse(apiId.ToString(), out eventId);
            using (var db = new DataContext())
            {
                var venue = db.Venues
                    .Where(v => v.BfEventId == eventId)
                    .FirstOrDefault();
                return venue;
            }
        }
    }
}

