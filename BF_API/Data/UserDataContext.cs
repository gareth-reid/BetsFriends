using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BF_API.Data
{
    public class UserDataContext : DbContext
    {        
        public UserDataContext()
        {

        }
        public DbSet<User> Users { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=tcp:betsfriendssql.database.windows.net,1433;Initial Catalog=BetsFriends;Persist Security Info=False;User ID=BetsFriends;Password=REDsky.123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
        }

        public override EntityEntry Add(object entity)
        {
            return base.Add(entity);
        }
    }    
}
