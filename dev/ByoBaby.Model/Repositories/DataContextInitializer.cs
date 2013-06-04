using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Security;

namespace ByoBaby.Model.Repositories
{
    public class ByoBabyDataContextInitializer  : DropCreateDatabaseAlways<ByoBabyRepository>
    {
        protected override void Seed(ByoBabyRepository context)
        {
            //context.People.Add(new Person()
            //{
            //    UserId = new Guid("98BFBB36-0742-41F0-818F-FC217B2E5553"),
            //    City = "Denver",
            //    Email = "nicknieslanik@gmail.com",
            //    FirstName = "Nick",
            //    LastName = "Nieslanik",
            //    Neighborhood = "Park Hill",
            //    LastUpdated = DateTime.Now,
            //    MemberSince = DateTime.Now,
            //    HomePhone = "720-939-9808",
            //    MobilePhone = "720-939-9808"
            //});
            //context.SaveChanges();
        }
    }
}