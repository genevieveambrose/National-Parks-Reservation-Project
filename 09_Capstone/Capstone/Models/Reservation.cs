using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone.Models
{
    public class Reservation
    {
        public int ReservationId { get; set; }
        public int SiteId { get; set; }
        public string Name { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public DateTime CreateDate { get; set; }
        public decimal DailyFee { get; set; }
        public string CampgroundName { get; set; }
        public int OpenMonth { get; set; }
        public int CloseMonth { get; set; }
    }
}
