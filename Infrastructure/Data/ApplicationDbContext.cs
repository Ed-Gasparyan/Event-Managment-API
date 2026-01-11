using Domain.Models;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Event> Events => Set<Event>();
    public DbSet<Venue> Venues => Set<Venue>();
    public DbSet<Ticket> Tickets => Set<Ticket>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // -----------------------------
        // Ticket → User relationship
        // One User can have many Tickets
        // Each Ticket belongs to exactly one User
        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.User)
            .WithMany(u => u.Tickets)
            .HasForeignKey(t => t.UserId);

        // -----------------------------
        // Ticket → Event relationship
        // One Event can have many Tickets
        // Each Ticket is for exactly one Event
        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.Event)
            .WithMany(e => e.Tickets)
            .HasForeignKey(t => t.EventId);

        // -----------------------------
        // Event → Venue relationship
        // One Venue can host many Events
        // Each Event takes place in exactly one Venue
        modelBuilder.Entity<Event>()
            .HasOne(e => e.Venue)
            .WithMany(v => v.Events)
            .HasForeignKey(e => e.VenueId);

        // -----------------------------
        // Ensure seat uniqueness per event
        // Prevents selling the same seat more than once for the same event
        modelBuilder.Entity<Ticket>()
            .HasIndex(t => new { t.EventId, t.SeatNumber })
            .IsUnique();
    }

}
