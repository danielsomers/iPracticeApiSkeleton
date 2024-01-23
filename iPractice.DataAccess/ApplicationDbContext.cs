using iPractice.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace iPractice.DataAccess
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Psychologist> Psychologists { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Availability> Availabilities { get; set; }
        public DbSet<TimeSlot> TimeSlots { get; set; }
        
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        { }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Psychologist>().HasKey(psychologist => psychologist.Id);
            modelBuilder.Entity<Client>().HasKey(client => client.Id);
            modelBuilder.Entity<Availability>().HasKey(availability => availability.Id);

            modelBuilder.Entity<Psychologist>()
                .HasMany(p => p.Clients)
                .WithMany(b => b.Psychologists);
            
            modelBuilder.Entity<Client>()
                .HasMany(p => p.Psychologists)
                .WithMany(b => b.Clients);
            
            modelBuilder.Entity<Psychologist>()
                .HasMany(a => a.Availabilities)
                .WithOne()
                .HasForeignKey(a => a.PsychologistId);
            
            modelBuilder.Entity<Availability>()
                .HasMany(a => a.TimeSlots)
                .WithOne(t => t.Availability)
                .HasForeignKey(t => t.AvailabilityId);
        }
    }
}