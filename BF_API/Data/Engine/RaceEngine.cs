using System;
using System.Collections.Generic;
using System.Linq;
using BetfairNG.Data;
using Microsoft.EntityFrameworkCore;

namespace BF_API.Data.Engine
{
    public class RaceEngine : IEngine<Race>
    {
        public bool Execute(List<MarketCatalogue> races, Venue venue)
        {
            try
            {
                using (var db = new DataContext())
                {
                    foreach (MarketCatalogue r in races)
                    {                        
                        var race = new Race
                        {
                            Description = r.MarketName,
                            BfMarketId = r.MarketId,
                            RaceTime = r.Description.MarketTime,
                            DateModified = DateTime.Now,
                            Venue = venue
                        };

                        var existingRace = GetFromApiId(r.MarketId);
                        db.Entry(race.Venue).State = EntityState.Unchanged;
                        if (existingRace != null)
                        {
                            race = Clone(race, existingRace);
                        }
                        else
                        {                            
                            db.Races.Add(race);
                        }                        
                        db.SaveChanges();
                    }
                }
                return true;
            } catch (Exception e)
            {
                return false;
            }
        }

        public Race Clone(Race newRace, Race existingRace)
        {         
            existingRace.Description = newRace.Description;
            existingRace.RaceTime = newRace.RaceTime;
            existingRace.DateModified = newRace.DateModified;
            existingRace.Venue = newRace.Venue;
            return existingRace;
        }

        public Race GetFromApiId(object apiId)
        {
            using (var db = new DataContext())
            {
                var race = db.Races
                    .Where(r => r.BfMarketId == apiId.ToString())
                    .FirstOrDefault();
                return race;
            }
        }
    }
}

