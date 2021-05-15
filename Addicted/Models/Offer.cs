using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Addicted.Models
{
    public class Offer
    {
        public int Id { get; set; }
        public Bet Bet { get; set; }
        public User User { get; set; }
        public BetOption BetOption { get; set; }
        public int Amount { get; set; }
    }
}
