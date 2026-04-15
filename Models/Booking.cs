namespace EventEase.Models
{

    //this is the booking models
    public class Booking
    {
        public int BookingId { get; set; }

        public int VenueId { get; set; }
        public Venue? Venue { get; set; }

        public int EventId { get; set; }
        public Event? Event { get; set; }

        public DateTime BookingDate { get; set; }

        public string? UniqueBookingCode { get; set; }

        public string? Status { get; set; }
    }
}
