using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Addicted.Authentication
{
    public interface IJwtAuthenticationManager
    {
        Task<JwtToken> Authenticate(string username, string password);
    }
}
