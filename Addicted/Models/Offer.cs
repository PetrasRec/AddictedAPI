using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Addicted.Models
{
    public class Offer
    {
        public int Id { get; set; }
        public Bet Bet { get; set; }
        public User User { get; set; }
        //public BetOption BetOption { get; set; }
        [ForeignKey("BetOptionId")]
        public int BetOptionId { get; set; }
        public int Amount { get; set; }
    }
}
