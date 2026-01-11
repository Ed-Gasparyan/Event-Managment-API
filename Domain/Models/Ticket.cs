using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public Event Event { get; set; } = null!;
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public string SeatNumber { get; set; } = null!;
        public DateTime PurchasedAt { get; set; }
        public decimal Price { get; set; }
    }

}
