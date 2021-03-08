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
        Task<IdentityResult> RegisterNewUser(UserModel user);
        Task<bool> LogInUser(string email, string password);
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

        public async Task<IdentityResult> RegisterNewUser(UserModel user)
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
                return result;
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
            return authenticationContext.users;
        }
    }
}
