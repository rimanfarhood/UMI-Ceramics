using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UmiCeramics.Models
{
    public class Booking
    {
        public int Id { get; set; }

        [Required]
        [MinLength(2)]
        public string CustomerName { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string CustomerEmail { get; set; } = string.Empty;

        public int NumberOfSeats { get; set; }

        public int WorkshopId { get; set; }

        public Workshop? Workshop { get; set; }
    }
}