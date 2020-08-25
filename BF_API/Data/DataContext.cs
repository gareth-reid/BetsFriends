using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BF_API.Data
{
    public class DataContext : DbContext
    {
        public DataContext() : base()
        {
            
            //Database.Se.SetInitializer(new DropCreateDatabaseAlways<UserDataContext>());
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Venue> Venues { get; set; }
        public DbSet<Race> Races { get; set; }
        public DbSet<Runner> Runners { get; set; }
        public DbSet<QuaddieGroup> Groups { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            #if DEBUG
            optionsBuilder.UseSqlServer("Server=tcp:localhost,1401;Initial Catalog=BetsFriends;Persist Security Info=False;User ID=SA;Password=REDsky.123;MultipleActiveResultSets=False;Connection Timeout=30;");
            #else
            optionsBuilder.UseSqlServer("Server=tcp:betsfriendssql.database.windows.net,1433;Initial Catalog=BetsFriends;Persist Security Info=False;User ID=BetsFriends;Password=REDsky.123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
            #endif
        }

        
        public override EntityEntry Add(object entity)
        {
            return base.Add(entity);
        }
    }    
}
