using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Addicted.Models
{
    [NotMapped]
    public class OfferModel
    {
        [Required]
        public int BetOptionId { get; set; }
        [Required]
        public int Amount { get; set; }
    }
}
