using System;
using System.Collections.Generic;
using System.Linq;
using BetfairNG.Data;
using BF_API.Data.Model;
using Microsoft.EntityFrameworkCore;

namespace BF_API.Data.Engine
{
    public class QuaddieBuilderEngine : IEngine<QuaddieGroup>
    {
        private RunnerEngine _runnerEngine = new RunnerEngine();
        private UserEngine _userEngine = new UserEngine();
        public bool Execute(string selectionId, string quaddieGroupId, string user)
        {
            try
            {
                using (var db = new DataContext())
                {
                    var runner = _runnerEngine.GetFromApiId(selectionId);
                    var quadddieGroup = GetFromApiId(quaddieGroupId);
                    if (runner != null)
                    {
                        var selectedRunner = new SelectedRunner();
                        selectedRunner.Runner = runner;
                        selectedRunner.User = _userEngine.GetUser(user);
                        selectedRunner.DateSelected = DateTime.Now;
                        db.Entry(selectedRunner.User).State = EntityState.Unchanged;
                        db.Entry(selectedRunner.Runner).State = EntityState.Unchanged;
                        if (quadddieGroup.QuaddieGroupId != 0)
                        {
                            db.Entry(quadddieGroup).State = EntityState.Modified;
                        }
                        else { //new
                            db.Entry(quadddieGroup).State = EntityState.Added;
                            quadddieGroup.Selections = new List<SelectedRunner>();
                        }
                        quadddieGroup.Selections.Add(selectedRunner);
                        //db.Groups.Add(quadddieGroup);                        
                        db.SaveChanges();
                    }
                }
                return true;
            } catch (Exception e)
            {
                return false;
            }
        }

        public QuaddieGroup GetFromApiId(object apiId)
        {
            var quaddieGroup = new QuaddieGroup();
            int quaddieGroupId;
            int.TryParse(apiId.ToString(), out quaddieGroupId);
            if (quaddieGroupId != 0)
            {
                using (var db = new DataContext())
                {                    
                    var quaddieGroups = db.Groups
                        .Include("Selections")
                        .Include("Selections.Runner")
                        .Include("Venue")
                        .Include("Selections.User")
                        .Where(qg => qg.QuaddieGroupId == quaddieGroupId).ToList();

                    if (quaddieGroups != null && quaddieGroups.Count > 0)
                    {
                        quaddieGroup = quaddieGroups                            
                            .FirstOrDefault();
                    }                    
                }
            }
            return quaddieGroup;
        }

        public QuaddieGroup Create(string description, string quaddieGroupId, string venueApiId, string user)
        {
            try
            {
                var quaddieGroup = GetFromApiId(quaddieGroupId);
                var venueEngine = new VenueEngine();
                var venue = venueEngine.GetFromApiId(venueApiId);
                var userEngine = new UserEngine();
                var activeUser = userEngine.GetUser(user);

                using (var db = new DataContext())
                {
                    quaddieGroup.Description = description;
                    quaddieGroup.Venue = venue;
                    db.Entry(quaddieGroup.Venue).State = EntityState.Unchanged;
                                        
                    if (quaddieGroup.QuaddieGroupId == 0)
                    {                        
                        db.Entry(quaddieGroup).State = EntityState.Added;                        
                    } else
                    {
                        db.Entry(quaddieGroup).State = EntityState.Modified;
                    }
                    quaddieGroup.AdminUser = activeUser;
                    db.Entry(quaddieGroup.AdminUser).State = EntityState.Unchanged;
                    db.SaveChanges();
                }

                return quaddieGroup;
            } catch (Exception e)
            {
                var ex = e;
                return null;
            }
        }

        //TODO: refactor to a toolbox
        public int GetInt(string value)
        {
            int valueAsInt;
            int.TryParse(value, out valueAsInt);
            return valueAsInt;

        }
        public List<QuaddieGroup> GetAll()
        {  
                using (var db = new DataContext())
                {                    
                    return db.Groups
                    .Where(qg => qg.Venue.OpenDate >= DateTime.Now)
                    .Include("Selections")
                    .Include("Selections.Runner")                    
                    .Include("Venue")
                    .Include("Selections.User")
                    .ToList();                    
                }
        }
        public QuaddieGroup Clone(QuaddieGroup newItem, QuaddieGroup existingItem)
        {
            throw new NotImplementedException();
        }
    }
}

