using HelpDesk.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesk.DAL.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TicketLog> TicketLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().Property(u => u.Role).HasConversion<string>();
            modelBuilder.Entity<Ticket>().Property(t => t.Category).HasConversion<string>();
            modelBuilder.Entity<Ticket>().Property(t => t.Priority).HasConversion<string>();
            modelBuilder.Entity<Ticket>().Property(t => t.Status).HasConversion<string>();
            modelBuilder.Entity<TicketLog>().Property(tl => tl.Action).HasConversion<string>();
            modelBuilder.Entity<TicketLog>().Property(tl => tl.FromStatus).HasConversion<string>();
            modelBuilder.Entity<TicketLog>().Property(tl => tl.ToStatus).HasConversion<string>();


            modelBuilder.Entity<Ticket>().HasOne(t => t.Submitter).WithMany(u => u.SubmittedTickets)
                                         .HasForeignKey(t => t.SubmitterId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Ticket>().HasOne(t => t.Assignee).WithMany(u => u.AssignedTickets)
                                         .HasForeignKey(t => t.AssigneeId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<User>().HasIndex(u => u.UserName).IsUnique();
            modelBuilder.Entity<TicketLog>().HasOne(l => l.Operator).WithMany(u => u.Logs)
                                            .HasForeignKey(l => l.OperatorId).OnDelete(DeleteBehavior.Restrict);

        }
    }
}
