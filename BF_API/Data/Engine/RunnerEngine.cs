using System;
using System.Collections.Generic;
using System.Linq;
using BetfairNG.Data;
using Microsoft.EntityFrameworkCore;

namespace BF_API.Data.Engine
{
    public class RunnerEngine : IEngine<Runner>
    {
        public bool Execute(List<RunnerCatalog> runners, List<BetfairNG.Data.Runner> runnersForMarket, Race race)
        {
            try
            {
                using (var db = new DataContext())
                {
                    foreach (RunnerCatalog rc in runners)
                    {
                        int runnerId;
                        int.TryParse(rc.SelectionId.ToString(), out runnerId);
                        var runner = new Runner
                        {
                            Race = race,
                            Name = rc.RunnerName,
                            BfSelectionId = runnerId,
                            DateModified = DateTime.Now,
                            LastPriceTraded = runnersForMarket.Find(rfm => rfm.SelectionId == rc.SelectionId).LastPriceTraded,
                            JockeyName = rc.Metadata.GetValueOrDefault("JOCKEY_NAME"),
                            TrainerName = rc.Metadata.GetValueOrDefault("TRAINER_NAME"),
                            Weight = rc.Metadata.GetValueOrDefault("WEIGHT_VALUE"),
                            Form = rc.Metadata.GetValueOrDefault("FORM"),
                            Barrier = rc.Metadata.GetValueOrDefault("STALL_DRAW")
                        };
                        var existingRunner = db.Runners
                            .Where(r => r.BfSelectionId == runnerId)
                           .FirstOrDefault();

                        db.Entry(runner.Race).State = EntityState.Unchanged;
                        if (existingRunner != null)
                        {
                            runner = Clone(runner, existingRunner);
                        }
                        else
                        {
                            db.Runners.Add(runner);
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

        public Runner Clone(Runner newRunner, Runner existingRunner)
        {
            existingRunner.Name = newRunner.Name;            
            existingRunner.DateModified = newRunner.DateModified;
            existingRunner.Race = newRunner.Race;

            existingRunner.LastPriceTraded = newRunner.LastPriceTraded;
            existingRunner.JockeyName = newRunner.JockeyName;
            existingRunner.TrainerName = newRunner.TrainerName;
            existingRunner.Weight = newRunner.Weight;
            existingRunner.Form = newRunner.Form;
            existingRunner.Barrier = newRunner.Barrier;
            return existingRunner;
        }

        public Runner GetFromApiId(object apiId)
        {
            int runnerId;
            int.TryParse(apiId.ToString(), out runnerId);
            using (var db = new DataContext())
            {
                var runner = db.Runners
                    .Where(r => r.BfSelectionId == runnerId)
                    .FirstOrDefault();
                return runner;
            }
        }
    }
}

