﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace EPS_Advance_Classes_Library.UserMgmt
{
    public class UserHandler : IDisposable
    {
        private readonly DBContextClass _db = new DBContextClass();

        public List<User> GetUsers()
        {
            using (_db)
            {
                return (from u in _db.Users
                        .Include("Role")
                        .Include("CityId.Country")
                        .Include("UserImage")
                        select u).ToList();
            }
        }

        public List<User> GetUsersByName()
        {
            using (_db)
            {
                return (from u in _db.Users.OrderBy(u => u.FullName)
                        .Include("Role")
                        .Include("CityId.Country")
                        .Include("UserImage")
                        select u).ToList();
            }
        }

        public User GetUserByEmail(string email)
        {
            using (_db)
            {
                return (from u in _db.Users
                        where u.Email == email
                        select u).FirstOrDefault();
            }
        }

        public User GetUser(int? id)
        {
            using (_db)
            {
                return (from u in _db.Users
                        .Include("Role")
                        .Include("CityId.Country")
                        .Include("UserImage")
                        where u.Id == id
                        select u).FirstOrDefault();
            }
        }

        public User GetUser(string loginId, string password)
        {
            using (_db)
            {
                return (from u in _db.Users
                        .Include("Role")
                        .Include("CityId.Country")
                        .Include("UserImage")
                        where (u.LoginID.Equals(loginId) || u.Email.Equals(loginId)) && u.Password.Equals(password)
                        select u).FirstOrDefault();
            }
        }

        public List<Role> GetRoles()
        {
            using (_db)
            {
                return (from r in _db.Roles select r).ToList();
            }
        }
        public Role GetRoles(int id)
        {
            using (_db)
            {
                return (from r in _db.Roles
                        where r.Id == id
                        select r).FirstOrDefault();
            }
        }

        public void Adduser(User user)
        {
            using (_db)
            {
                _db.Entry(user.Role).State = EntityState.Unchanged;
                _db.Entry(user.CityId).State = EntityState.Unchanged;
                _db.Users.Add(user);
                _db.SaveChanges();
            }
        }

        public void DeleteUser(int id)
        {
            using (_db)
            {
                //if any error occured just Uncomment this code

                User u = _db.Users.Find(id);
                _db.Users.Remove(u);
                _db.SaveChanges();
            }
        }

        public void UpdateUser(User user)
        {
            using (_db)
            {
                _db.Entry(user).State = EntityState.Modified;
                _db.SaveChanges();
            }
        }

        public int GetUserCount()
        {
            using (_db)
            {
                return (from c in _db.Users select c).Count();
            }
        }

        public void UpdateUserByAdmin(User newUser)
        {

            User oldUser = _db.Users.Include(i => i.CityId).Include(i => i.Role).SingleOrDefault(x => x.Id == newUser.Id);

            if (oldUser != null)
            {

                //if (newUser.CityId.Id != 0 || newUser.CityId != null)
                //{
                //    oldUser.CityId = newUser.CityId;
                //}


                oldUser.FullName = newUser.FullName;
                oldUser.LoginID = newUser.LoginID;
                oldUser.BirthDate = newUser.BirthDate;
                oldUser.Email = newUser.Email;
                oldUser.FullAddress = newUser.FullAddress;
                oldUser.Phone = newUser.Phone;
                oldUser.SecurityQuestion = newUser.SecurityQuestion;
                oldUser.SecurityAnswer = newUser.SecurityAnswer;
                oldUser.UserImage = newUser.UserImage;
                oldUser.IsActive = newUser.IsActive;
            }

            _db.Entry(oldUser.Role).State = EntityState.Unchanged;
            _db.Entry(oldUser.CityId).State = EntityState.Unchanged;
            _db.Entry(oldUser).State = EntityState.Modified;
            _db.SaveChanges();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
        }

    }
}
