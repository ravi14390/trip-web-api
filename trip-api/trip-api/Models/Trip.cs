using System;
namespace trip_api.Models
{
    public class Trip
    {
        
        public string Name { get; set; }

        public string id { get; set; }

        public string FromCountry { get; set; }

        public string ToCountry { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool Cancelled { get; set; }

        public string CustomerId { get; set; }
    }
}
