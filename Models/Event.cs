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

        public int? EventTypeId { get; set; }
        public EventType? EventType { get; set; }

        public ICollection<Booking>? Bookings { get; set; }
    }
}