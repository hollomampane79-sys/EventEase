namespace EventEase.Models
{
    public class Venue
    {
        public int VenueId { get; set; }

        public string? Name { get; set; }

        public string? Location { get; set; }

        public int Capacity { get; set; }

        public string? ImageUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<Booking>? Bookings { get; set; }
    }
}