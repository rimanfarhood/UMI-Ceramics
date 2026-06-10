using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UmiCeramics.Models
{
    public class Booking
    {
        public int Id { get; set; }

        public required string CustomerName { get; set; }

        public required string CustomerEmail { get; set; }

        public int NumberOfSeats { get; set; }

        public int WorkshopId { get; set; }

        public Workshop? Workshop { get; set; }
    }
}