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
        Task<UserModel> AddNewUser(UserModel user);
        Task<bool> LogInUser(string email, string password);
        Task<UserModel> UpdateUserByID(string id, UserModel newData);
        User GetUserByEmail(string id);
        User GetUserById(string id);
        IEnumerable<User> GetAllUsers();
        Task<string> GetUserRoleId(User user);
    }

    public class UsersService : IUsersService
    {
        private UserManager<User> _userManager;
        private SignInManager<User> _signInManager;
        private AuthenticationContext authenticationContext;
        public RoleManager<IdentityRole> _rolesManager;
        public UsersService(UserManager<User> userManager, SignInManager<User> signInmanager, AuthenticationContext authenticationContext, RoleManager<IdentityRole> rolesManager)
        {
            this._signInManager = signInmanager;
            this._userManager = userManager;
            this.authenticationContext = authenticationContext;
            this._rolesManager = rolesManager;
        }

        public async Task<bool> LogInUser(string email, string password)
        {  
            var result = await _signInManager.PasswordSignInAsync(email, password, true, false);
            return result.Succeeded;
        }
        public async Task<string> GetUserRoleId(User user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            string role = roles.SingleOrDefault();
            return _rolesManager.Roles.Single(r => r.Name == role).Id;
        }
        public async Task<UserModel> UpdateUserByID(string id, UserModel newData)
        {
            var user = authenticationContext.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return null;
            }
            user.Name = newData.Name;
            user.Surname = newData.Surname;
            user.Email = newData.Email;
            user.UserName = newData.UserName;
            var roles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, roles);

            var role = _rolesManager.Roles.Single(r => r.Id == newData.RoleId);

            await _userManager.AddToRoleAsync(user, role.Name);

            authenticationContext.SaveChanges();
            return new UserModel
            {
                Id = user.Id,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                UserName = user.UserName,
                RoleId = role.Id,
            };
        }

        public async Task<User> RegisterNewUser(UserModel user)
        {
            var addedUser = new User()
            {
                UserName = user.Email,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                Coins = new Coin { VaciusCoin=100 }
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

        public async Task<UserModel> AddNewUser(UserModel user)
        {
            var addedUser = new User()
            {
                UserName = user.Email,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                Coins = new Coin { VaciusCoin = 100 }
            };
            try
            {
                var result = await _userManager.CreateAsync(addedUser, user.Password);
                if (!result.Succeeded)
                {
                    return null;
                }
                var newUser = authenticationContext.Users.Single(u => addedUser.Email.ToUpper() == u.NormalizedEmail);

                var roleToBeAdded = _rolesManager.Roles.SingleOrDefault(role => role.Id == user.RoleId);

                await _userManager.AddToRoleAsync(newUser, roleToBeAdded.Name);

                return new UserModel
                {
                    Id = newUser.Id,
                    Name = newUser.Name,
                    Surname = newUser.Surname,
                    Email = newUser.Email,
                    UserName = newUser.UserName,
                    RoleId = roleToBeAdded.Id,
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public User GetUserById(string id)
        {
            return authenticationContext.Users.SingleOrDefault((user) => user.Id == id);
        }
        public User GetUserByEmail(string email)
        {
            return authenticationContext.GetAllUsers().SingleOrDefault(e => e.Email == email);
        }
        public IEnumerable<User> GetAllUsers()
        {
            return authenticationContext.Users;
        }
    }
}
