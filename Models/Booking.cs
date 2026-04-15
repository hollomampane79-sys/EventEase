namespace EventEase.Models
{

    //this is the booking models which will be used to create the booking table in the database and also to create the booking controller and views
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
