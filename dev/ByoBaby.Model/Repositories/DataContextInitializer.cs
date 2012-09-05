using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Security;

namespace ByoBaby.Model.Repositories
{ 
    public class DataContextInitializer : DropCreateDatabaseAlways<ByoBabyRepository>
    {
        protected override void Seed(ByoBabyRepository context)
        {
        MembershipCreateStatus Status;
        Membership.CreateUser("Demo", "123456", "demo@demo.com", null, null, true, out Status);
        Roles.CreateRole("Admin");
        Roles.AddUserToRole("Demo", "Admin");
        }
    }
}