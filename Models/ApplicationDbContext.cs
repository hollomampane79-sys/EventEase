using Microsoft.EntityFrameworkCore;

namespace EventEase.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Venue> Venues { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<EventType> EventTypes { get; set; }  // NEW

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Seed predefined EventType categories
            modelBuilder.Entity<EventType>().HasData(
                new EventType { EventTypeId = 1, Name = "Conference" },
                new EventType { EventTypeId = 2, Name = "Wedding" },
                new EventType { EventTypeId = 3, Name = "Concert" },
                new EventType { EventTypeId = 4, Name = "Corporate Function" },
                new EventType { EventTypeId = 5, Name = "Birthday Party" },
                new EventType { EventTypeId = 6, Name = "Exhibition" },
                new EventType { EventTypeId = 7, Name = "Workshop" },
                new EventType { EventTypeId = 8, Name = "Other" }
            );
        }
    }
}