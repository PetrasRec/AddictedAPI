using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Addicted.Models
{
    public class User : IdentityUser
    {
        [Column(TypeName = "nvarchar(150)")]
        public string Name { get; set; }

        [Column(TypeName = "nvarchar(150)")]
        public string Surname { get; set; }
        public Coin Coins { get; set; }
    }
}
