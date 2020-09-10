using System;
using System.Linq;

namespace BF_API.Data.Engine
{
    public class UserEngine
    {
        public UserEngine()
        {
        }

        public bool AddUser(string username, string password, string email)
        {
            using (var db = new DataContext())
            {
                var existingUser = db.Users
                    .Where(u => u.Name == username)
                   .FirstOrDefault();
                if (existingUser == null)
                {
                    var user = new User
                    {
                        Name = username,
                        Password = password,
                        Email = email,
                        DateAdded = DateTime.Now
                    };
                    db.Users.Add(user);
                    db.SaveChanges();
                    return true;
                } else
                {
                    return false;
                }
            }
        }

        public string GetUser(string username, string password)
        {
            using (var db = new DataContext())
            {
                var existingUser = db.Users
                    .Where(u => u.Email == username
                    && u.Password == password)
                   .FirstOrDefault();
                if (existingUser != null)
                {
                    return existingUser.Name;
                }
                else
                {
                    return "";
                }
            }
        }

        public User GetUser(string username)
        {
            using (var db = new DataContext())
            {
                var user = db.Users
                    .Where(u => u.Name == username)
                   .FirstOrDefault();
                return user;
            }
        }


    }
}
