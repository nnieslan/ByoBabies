using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByoBaby.Model.Repositories
{
    public class ByoBabyRepository : DbContext
    {
        public DbSet<Person> People { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Child> Children { get; set; }
        public DbSet<Blurb> Blurbs { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<CheckIn> CheckIns { get; set; }

        public DbSet<Setting> Settings { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Child>().HasRequired(e => e.Parent).WithMany(e => e.Children);

            modelBuilder.Entity<Person>().HasMany(e => e.Friends).WithMany();
            modelBuilder.Entity<Person>().HasMany(e => e.MemberOf);
            modelBuilder.Entity<Person>().HasMany(e => e.Notifications);
            modelBuilder.Entity<Person>().HasMany(e => e.PendingRequests);
            modelBuilder.Entity<Notification>().HasRequired(e => e.Originator);
            modelBuilder.Entity<CheckIn>().HasRequired(e => e.Owner);
        }
    }
}
