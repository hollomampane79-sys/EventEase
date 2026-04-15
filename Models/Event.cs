namespace EventEase.Models
{
    public class Event
    {
        public int EventId { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public DateTime CreatedAt { get; set; }

        public ICollection<Booking>? Bookings { get; set; }
    }
}
