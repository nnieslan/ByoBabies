using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Security;

namespace ByoBaby.Model.Repositories
{
    public class ByoBabyDataContextInitializer : DropCreateDatabaseAlways<ByoBabyRepository>
    {
        protected override void Seed(ByoBabyRepository context)
        {
            Person nickProfile = null;
            Person tiffanyProfile = null;
            Person willProfile = null;

            //create a stubbed in profile for the newly logged in user
            using (aspnet_fbaEntities1 entityContext = new aspnet_fbaEntities1())
            {
                var nick = entityContext.aspnet_Users.FirstOrDefault(p => p.UserName == "nicknieslanik@gmail.com");
                if (nick != null)
                {
                    nickProfile = new Person()
                    {
                        City = "Denver",
                        State = "CO",
                        Email = "nicknieslanik@gmail.com",
                        ProfilePictureUrl = "http://m.c.lnkd.licdn.com/mpr/mpr/shrink_200_200/p/3/000/029/338/0961cc9.jpg",
                        FirstName = "Nick",
                        LastName = "Nieslanik",
                        Neighborhood = "Park Hill",
                        UserId = nick.UserId,
                        HomePhone = "720-939-9808",
                        MobilePhone = "720-939-9808",
                        MemberSince = DateTime.Now,
                        LastUpdated = DateTime.Now

                    };

                    context.People.Add(nickProfile);
                    context.SaveChanges();
                    nickProfile.Children = new Collection<Child>()
                    {
                        new Child() { ParentId = nickProfile.Id, Name="Ephraim", Age=1, Gender = "M"} 
                    };
                    context.SaveChanges();
                }

                var tiffany = entityContext.aspnet_Users.FirstOrDefault(p => p.UserName == "tiffanynieslanik@gmail.com");
                if (tiffany != null)
                {
                    tiffanyProfile = new Person()
                    {
                        City = "Denver",
                        State = "CO",
                        Email = "tiffanynieslanik@gmail.com",
                        FirstName = "Tiffany",
                        LastName = "Nieslanik",
                        Neighborhood = "Park Hill",
                        UserId = tiffany.UserId,
                        HomePhone = "303-819-4661",
                        MobilePhone = "303-819-4661",
                        MemberSince = DateTime.Now,
                        LastUpdated = DateTime.Now

                    };
                    context.People.Add(tiffanyProfile);
                    context.SaveChanges();
                    tiffanyProfile.Children = new Collection<Child>()
                    {
                        new Child() { ParentId = tiffanyProfile.Id, Name="Ephraim", Age=1, Gender = "M"} 
                    };
                    context.SaveChanges();
                }

                var will = entityContext.aspnet_Users.FirstOrDefault(p => p.UserName == "w.simpson@hotmail.com");
                if (will != null)
                {
                    willProfile = new Person()
                    {
                        City = "Denver",
                        State = "CO",
                        Email = "w.simpson@hotmail.com",
                        FirstName = "Will",
                        LastName = "Simpson",
                        Neighborhood = "North Park Hill",
                        UserId = will.UserId,
                        HomePhone = "720-884-7684",
                        MobilePhone = "720-884-7684",
                        MemberSince = DateTime.Now,
                        LastUpdated = DateTime.Now

                    };
                    context.People.Add(willProfile);
                    context.SaveChanges();
                    willProfile.Children = new Collection<Child>()
                    {
                        new Child() { ParentId = willProfile.Id, Name="Jude", Age=1, Gender = "M"} 
                    };
                    context.SaveChanges();
                }


                if (tiffanyProfile != null && nickProfile != null)
                {
                    nickProfile.Friends = new Collection<Person>() { tiffanyProfile };
                    tiffanyProfile.Friends = new Collection<Person>() { nickProfile };

                    context.SaveChanges();  
                }

                if (willProfile != null && nickProfile != null)
                {

                    var fr = new FriendRequest()
                     {
                         Title = "Wait a minute.... You have a kid too?",
                         Description = "Hi Guy! I'd like to hang-out, play-date and stuff.",
                         RequestorId = willProfile.Id,
                         TargetId = nickProfile.Id
                     };

                    nickProfile.PendingRequests = new Collection<Request>() { fr };
                    nickProfile.Notifications = new Collection<Notification>() { new Notification() { Originator = fr } };

                    context.SaveChanges();
                }
            }
        }
    }
}