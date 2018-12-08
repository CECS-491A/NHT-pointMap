﻿using DataAccessLayer.Database;
using DataAccessLayer.Models;
using ServiceLayer.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerLayer.UserManagement
{
    public class UserManagementManager
    {
        private IPasswordService _passwordService;
        private IUserService _userService;

        private DatabaseContext CreateDbContext()
        {
            return new DatabaseContext();
        }

        public int CreateUser(string email, string password, DateTime dob)
        {
            DateTime timestamp = DateTime.UtcNow;
            _passwordService = new PasswordService();
            _userService = new UserService();
            byte[] salt = _passwordService.GenerateSalt();
            string hash = _passwordService.HashPassword(password, salt);
            User user = new User
            {
                Email = email,
                PasswordHash = hash,
                PasswordSalt = salt,
                DateOfBirth = dob,
                UpdatedAt = timestamp
            };

            using (var _db = CreateDbContext())
            {
                var response = _userService.CreateUser(user, _db);
                try
                {
                    return _db.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    //catch error
                    // detach user attempted to be created from the db context - rollback
                    _db.Entry(response).State = System.Data.Entity.EntityState.Detached;
                }
            }
            return 0;
        }

        public int DeleteUser(User user)
        {
            _userService = new UserService();
            using (var _db = CreateDbContext())
            {
                var response = _userService.DeleteUser(user.Id, _db);
                // will return null if user does not exist
                return _db.SaveChanges();
            }
        }

        public int DeleteUser(Guid id)
        {
            _userService = new UserService();
            using (var _db = CreateDbContext())
            {
                var response = _userService.DeleteUser(id, _db);
                return _db.SaveChanges();
            }
        }

        public User GetUser(Guid id)
        {
            _userService = new UserService();
            using (var _db = CreateDbContext())
            {
                return _userService.GetUser(id, _db);
            }
        }

        public User GetUser(string email)
        {
            _userService = new UserService();
            using (var _db = CreateDbContext())
            {
                return  _userService.GetUser(email, _db);
            }
        }

        public int DisableUser(User user)
        {
            using (var _db = CreateDbContext())
            {
                user.Disabled = true;
                var response = _userService.UpdateUser(user, _db);
                return _db.SaveChanges();
            }
        }

        public int EnableUser(User user)
        {
            using (var _db = CreateDbContext())
            {
                user.Disabled = false;
                var response = _userService.UpdateUser(user, _db);
                return _db.SaveChanges();
            }
        }

        public int ToggleUser(User user)
        {
            using (var _db = CreateDbContext())
            {
                user.Disabled = !user.Disabled;
                var response = _userService.UpdateUser(user, _db);
                return _db.SaveChanges();
            }
        }

        public int UpdateUser(User user)
        {
            using (var _db = CreateDbContext())
            {
                var response = _userService.UpdateUser(user, _db);
                try
                {
                    return _db.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    // catch error
                    // rollback changes
                    _db.Entry(response).CurrentValues.SetValues(_db.Entry(response).OriginalValues);
                    _db.Entry(response).State = System.Data.Entity.EntityState.Unchanged;
                    return 0;
                }
            }
        }
    }
}
