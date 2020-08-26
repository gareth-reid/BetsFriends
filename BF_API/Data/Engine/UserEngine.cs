﻿using System;
using System.Linq;

namespace BF_API.Data.Engine
{
    public class UserEngine
    {
        public UserEngine()
        {
        }

        public void AddUser(string username, string password, string email)
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

        
    }
}