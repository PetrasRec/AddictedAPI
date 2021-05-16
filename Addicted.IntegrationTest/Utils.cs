using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Addicted.Models;

namespace Addicted.IntegrationTests
{
    public class Utils
    {
        public static User GenerateUser(string username)
        {
            return new User
            {
                Email = $"{username}@{username}.{username}",
                Name = username,
            };
        }
    }
}
