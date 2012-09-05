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
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Child> Children { get; set; }
        public DbSet<Blurb> Blurbs { get; set; }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<SecurityRole> SecurityRoles { get; set; }
        public DbSet<SecurityRoleUser> SecurityRoleUsers { get; set; }

        public DbSet<Setting> Settings { get; set; }
    }
}
