using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

             //create a stubbed in profile for the newly logged in user
            using (aspnet_fbaEntities1 entityContext = new aspnet_fbaEntities1())
            {
                var user = entityContext.aspnet_Users.FirstOrDefault(p => p.UserName == "nicknieslanik@gmail.com");
                if (user != null)
                {
                    var profile = new Person()
                    {
                        City = "Denver",
                        State = "CO",
                        Email = "nicknieslanik@gmail.com",
                        FirstName = "Nick",
                        LastName = "Nieslanik",
                        Neighborhood = "Park Hill",
                        UserId = user.UserId,
                        HomePhone = "720-939-9808",
                        MobilePhone = "720-939-9808",
                        MemberSince = DateTime.Now,
                        LastUpdated = DateTime.Now

                    };
                    context.People.Add(profile);
                    context.SaveChanges();
                    profile.Children = new Collection<Child>()
                    {
                        new Child() { ParentId = profile.Id, Name="Ephraim", Age=1, Gender = "M"} 
                    };
                    context.SaveChanges();
                }
            }
        }
    }
}