using Addicted.Config;
using Addicted.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Addicted.Service
{
    public interface IUsersService
    {
        Task<User> RegisterNewUser(UserModel user);
        Task<User> AddNewUser(UserModel user);
        Task<bool> LogInUser(string email, string password);
        User UpdateUserByID(string id, UserModel newData);
        User GetUserByEmail(string id);
        IEnumerable<User> GetAllUsers();
    }

    public class UsersService : IUsersService
    {
        private UserManager<User> _userManager;
        private SignInManager<User> _signInManager;
        private AuthenticationContext authenticationContext;
        public UsersService(UserManager<User> userManager, SignInManager<User> signInmanager, AuthenticationContext authenticationContext)
        {
            this._signInManager = signInmanager;
            this._userManager = userManager;
            this.authenticationContext = authenticationContext;
        }

        public async Task<bool> LogInUser(string email, string password)
        {  
            var result = await _signInManager.PasswordSignInAsync(email, password, true, false);
            return result.Succeeded;
        }

        public User UpdateUserByID(string id, UserModel newData)
        {
            var user = authenticationContext.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return null;
            }
            user.Name = newData.Name;
            user.Surname = newData.Surname;
            authenticationContext.SaveChanges();
            return user;
        }

        public async Task<User> RegisterNewUser(UserModel user)
        {
            var addedUser = new User()
            {
                UserName = user.Email,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
            };
            try
            {
                var result = await _userManager.CreateAsync(addedUser, user.Password);
                if (!result.Succeeded)
                {
                    return null;
                }
                var newUser = authenticationContext.Users.Single(u => addedUser.Email.ToUpper() == u.NormalizedEmail);

                await _userManager.AddToRoleAsync(newUser, Roles.User);
                return newUser;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<User> AddNewUser(UserModel user)
        {
            var addedUser = new User()
            {
                UserName = user.Email,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
            };
            try
            {
                var result = await _userManager.CreateAsync(addedUser, user.Password);
                if (!result.Succeeded)
                {
                    return null;
                }
                var newUser = authenticationContext.Users.Single(u => addedUser.Email.ToUpper() == u.NormalizedEmail);

                await _userManager.AddToRoleAsync(newUser, Roles.User);
                return newUser;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public User GetUserByEmail(string email)
        {
            return authenticationContext.GetUserByEmail(email);
        }

        public IEnumerable<User> GetAllUsers()
        {
            return authenticationContext.Users;
        }
    }
}
