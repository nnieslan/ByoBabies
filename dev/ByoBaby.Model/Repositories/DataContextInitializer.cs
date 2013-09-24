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
            var appId = new Guid("48809099-2A86-4DB1-A6A1-CB7BA1244653");

            //create a stubbed in profile for the newly logged in user
            using (aspnet_fbaEntities1 entityContext = new aspnet_fbaEntities1())
            {
                if (entityContext.aspnet_Applications.FirstOrDefault() == null)
                {
                    entityContext.aspnet_Applications.Add(new aspnet_Applications() { ApplicationId = new Guid("48809099-2A86-4DB1-A6A1-CB7BA1244653"), ApplicationName = "/", LoweredApplicationName = "/" });
                    entityContext.SaveChanges();
                }

                Person nick = SeedPerson(
                    entityContext,
                    context,
                    appId,
                    new Guid("B610D6BC-767D-47E0-9951-231AE4AD02E2"),
                    "J0Wk3FTnzniXyp/rn7r37acaYRE=",
                    "hxafmFWaLVRR+/cWtViUdw==",
                    "nicknieslanik@gmail.com",
                    "Nick",
                    "Nieslanik",
                    "720-939-9808",
                    "Park Hill",
                    "http://m.c.lnkd.licdn.com/mpr/mpr/shrink_200_200/p/3/000/029/338/0961cc9.jpg",
                   "Denver", "CO",
                    new Collection<Child>() { new Child() { Name = "Ephraim", Age = 1, Gender = "M" } }
                );

                Person tiffany = SeedPerson(
                    entityContext,
                    context,
                    appId,
                    new Guid("734B0ACE-A910-4966-BD15-A078E229D0A6"),
                    "ueQv9XM9kmKdN9Qmbcl/dRk5+2g=",
                    "vkVlI9Qq6XVy+x2l3g0Mww==",
                    "tiffanynieslanik@gmail.com",
                    "Tiffany",
                    "Nieslanik",
                    "303-819-4661",
                    "Park Hill",
                    null,
                   "Denver", "CO",
                    new Collection<Child>() { new Child() { Name = "Ephraim", Age = 1, Gender = "M" } }
                );

                Person will = SeedPerson(
                    entityContext,
                    context,
                    appId,
                    new Guid("FC0BDAB5-384A-4F82-8633-B08A2882CA0D"),
                    "MPj9mB+ZgBRc/eBZ6S4LKc+h4mg=",
                    "2yQrfoU7gozP4Ea0e/Xg/Q==",
                    "w.simpson@hotmail.com",
                    "Will",
                    "Simpson",
                    "720-884-7684",
                    "North Park Hill",
                    null,
                   "Denver", "CO",
                    new Collection<Child>() { new Child() { Name = "Jude", Age = 1, Gender = "M" } }
                );

                Person julie = SeedPerson(
                   entityContext,
                   context,
                   appId,
                   new Guid("ECDDD77B-396A-4357-951A-199296DA12FB"),
                   "MPj9mB+ZgBRc/eBZ6S4LKc+h4mg=",
                   "2yQrfoU7gozP4Ea0e/Xg/Q==",
                   "brljcsus@msn.com",
                   "Julie",
                   "Simpson",
                   "720-315-6389",
                   "North Park Hill",
                   null,
                   "Denver", "CO",
                   new Collection<Child>() { new Child() { Name = "Jude", Age = 1, Gender = "M" } }
               );

                Person pete = SeedPerson(
                   entityContext,
                   context,
                   appId,
                   new Guid("75B218F9-4946-4C5C-8234-9B06FAC7D96E"),
                   "MPj9mB+ZgBRc/eBZ6S4LKc+h4mg=",
                   "2yQrfoU7gozP4Ea0e/Xg/Q==",
                   "pbonkrude@gmail.com",
                   "Peter",
                   "Bonkrude",
                   "720-839-6232",
                   "",
                   null, 
                   "Redding",
                   "CA",
                   new Collection<Child>() { new Child() { Name = "Linnea", Age = 1, Gender = "F" } }
               );

                Person nicole = SeedPerson(
                   entityContext,
                   context,
                   appId,
                   new Guid("16FDD18D-EE7C-4805-8AC1-22EB93265BB1"),
                   "MPj9mB+ZgBRc/eBZ6S4LKc+h4mg=",
                   "2yQrfoU7gozP4Ea0e/Xg/Q==",
                   "nbonkrude@gmail.com",
                   "Nicole",
                   "Bonkrude",
                   "530-339-6833",
                   "",
                   null,
                   "Redding", 
                   "CA",
                   new Collection<Child>() { new Child() { Name = "Linnea", Age = 1, Gender = "F" } }
               );


                Person pat = SeedPerson(
                   entityContext,
                   context,
                   appId,
                   new Guid("9F9F32A1-35C5-4133-8D82-E8A30CAF4CFB"),
                   "MPj9mB+ZgBRc/eBZ6S4LKc+h4mg=",
                   "2yQrfoU7gozP4Ea0e/Xg/Q==",
                   "patrickjbowden@gmail.com",
                   "Pat",
                   "Bowden",
                   "303-809-0649",
                   "",
                   null,
                   "Charlotte",
                   "NC",
                   new Collection<Child>() { new Child() { Name = "Wynne", Age = 1, Gender = "F" } }
               );

                Person meghan = SeedPerson(
                   entityContext,
                   context,
                   appId,
                   new Guid("41993CBF-DF08-48E3-9476-43C7D73A258C"),
                   "MPj9mB+ZgBRc/eBZ6S4LKc+h4mg=",
                   "2yQrfoU7gozP4Ea0e/Xg/Q==",
                   "meghanrbowden@gmail.com",
                   "Meghan",
                   "Bowden",
                   "303-564-8112",
                   "",
                   null,
                   "Charlotte",
                   "NC",
                   new Collection<Child>() { new Child() { Name = "Wynne", Age = 1, Gender = "F" } }
               );

                nick.Friends = new Collection<Person>() { tiffany, pat, pete, meghan};
                tiffany.Friends = new Collection<Person>() { nick };
                pat.Friends = new Collection<Person>() { nick, meghan };
                pete.Friends = new Collection<Person>() { nick, nicole };
                nicole.Friends = new Collection<Person>() { nick, pete};
                meghan.Friends = new Collection<Person>() { nick, pat};
                will.Friends = new Collection<Person>() { julie };
                julie.Friends = new Collection<Person>() { will };
                context.SaveChanges();

                var fr1 = new FriendRequest()
                 {
                     Title = "Wait a minute.... You have a kid too?",
                     Description = "Hi Guy! I'd like to hang-out, play-date and stuff.",
                     Requestor = will,
                     Audience = nick
                 };
                var fr2 = new FriendRequest()
                {
                    Title = "It's called ---Orange Crush---",
                    Description = "Seriously! I'd like to hang-out, play-date and stuff.",
                    Requestor = julie,
                    Audience = nick
                };

                nick.PendingRequests = new Collection<Request>() { fr1, fr2 };
                nick.Notifications = new Collection<Notification>() { new Notification() { Originator = fr1 }, new Notification() {Originator = fr2} };

                context.SaveChanges();

                //w-h-ferguson-park-denver - Lat: 39.7510396689177 | Lon: -104.932819828391
                //spinellis-market-denver - Lat: 39.7510845959187 | Lon: -104.933606386185
                //cherry-tomato-denver - Lat: 39.7510845959187 | Lon: -104.933419302106
                //parkhill-dental-arts-denver - Lat: 39.7509947419167 | Lon: -104.933574870229
                //oonas-dog-groom-and-spa-denver - Lat: 39.7512626647949 | Lon: -104.933486938477
                //park-hill-community-book-store-denver - Lat: 39.7508087158203 | Lon: -104.933151245117
                //moss-pink-flora-and-botanicals-denver - Lat: 39.7510845959187 | Lon: -104.93365265429
                //best-friends-forever-pet-care-denver - Lat: 39.7510845959187 | Lon: -104.933668747544
                //park-hill-cleaners-and-tailors-denver - Lat: 39.7509947419167 | Lon: -104.933668747544
                //adagio-baking-company-denver - Lat: 39.7509947419167 | Lon: -104.933544024825
                //montgomery-photography-denver-2 - Lat: 39.7548789978027 | Lon: -104.933135986328
                //park-properties-realty-denver - Lat: 39.752685546875 | Lon: -104.926094055176
                //park-hill-branch-library-denver - Lat: 39.7477760314941 | Lon: -104.932525634766
                //eis-gelato-denver - Lat: 39.758228302002 | Lon: -104.928550720215
                //the-bike-depot-denver - Lat: 39.7572631835938 | Lon: -104.928558349609
                //tables-denver - Lat: 39.7504308074713 | Lon: -104.917591586709
                //cijis-natural-pet-supplies-denver - Lat: 39.750337600708 | Lon: -104.917473569512
                //green-buddies-usa-denver - Lat: 39.7404022216797 | Lon: -104.94246673584
                //park-hill-veterinary-medical-center-denver - Lat: 39.7504515945911 | Lon: -104.908188432455
                //babooshka-denver-2 - Lat: 39.7403717041016 | Lon: -104.949234008789


                context.CheckIns.Add(new CheckIn() { Owner = tiffany, Duration = 60, LocationId = "w-h-ferguson-park-denver", Latitude=39.7510396689177, Longitude = -104.932819828391, StartTime = System.DateTime.Now });
                context.CheckIns.Add(new CheckIn() { Owner = will, Duration = 30, LocationId = "spinellis-market-denver", Latitude=39.7510845959187, Longitude = -104.933606386185, StartTime = System.DateTime.Now });
                context.CheckIns.Add(new CheckIn() { Owner = julie, Duration = 30, LocationId = "spinellis-market-denver", Latitude=39.7510845959187, Longitude = -104.933606386185, StartTime = System.DateTime.Now });
                context.CheckIns.Add(new CheckIn() { Owner = pete, Duration = 30, LocationId = "adagio-baking-company-denver", Latitude=39.7509947419167, Longitude = -104.933544024825, StartTime = System.DateTime.Now });
                context.CheckIns.Add(new CheckIn() { Owner = pat, Duration = 90, LocationId = "eis-gelato-denver", Latitude=39.758228302002, Longitude = -104.928550720215, StartTime = System.DateTime.Now });
                context.CheckIns.Add(new CheckIn() { Owner = meghan, Duration = 90, LocationId = "tables-denver", Latitude=39.7504308074713, Longitude = -104.917591586709, StartTime = System.DateTime.Now });
                context.CheckIns.Add(new CheckIn() { Owner = nicole, Duration = 120, LocationId = "park-hill-veterinary-medical-center-denver", Latitude = 39.7504515945911, Longitude = -104.949234008789, StartTime = System.DateTime.Now });

            }

        }
        private static Person SeedPerson(
            aspnet_fbaEntities1 entityContext,
            ByoBabyRepository context,
            Guid applicationId,
            Guid userId,
            string password,
            string passwordSalt,
            string email,
            string firstName,
            string lastName,
            string phone,
            string neighborhood,
            string profilePic,
            string city,
            string state,
            Collection<Child> children
            )
        {
            var user = entityContext.aspnet_Users.FirstOrDefault(p => p.UserName == email);
            if (user == null)
            {
                //user insert
                user = new aspnet_Users() { ApplicationId = applicationId, UserId = userId, UserName = email, LoweredUserName = email, LastActivityDate = System.DateTime.Now.Date };
                entityContext.aspnet_Users.Add(user);
                //me-bership insert
                var nickMembership = new aspnet_Membership() { ApplicationId = applicationId, UserId = userId, Password = password, PasswordFormat = 1, PasswordSalt = passwordSalt, Email = email, LoweredEmail = email, IsApproved = true, IsLockedOut = false, FailedPasswordAttemptCount = 0, FailedPasswordAnswerAttemptCount = 0, LastLockoutDate = new DateTime(1753, 1, 1), LastLoginDate = new DateTime(1753, 1, 1), LastPasswordChangedDate = new DateTime(1753, 1, 1), CreateDate = DateTime.Now.Date, FailedPasswordAnswerAttemptWindowStart = new DateTime(1753, 1, 1), FailedPasswordAttemptWindowStart = new DateTime(1753, 1, 1) };
                entityContext.aspnet_Membership.Add(nickMembership);
                entityContext.SaveChanges();
            }
            var userProfile = new Person()
            {
                City = city,
                State = state,
                Email = email,
                ProfilePictureUrl = profilePic,
                FirstName = firstName,
                LastName = lastName,
                Neighborhood = neighborhood,
                UserId = user.UserId,
                HomePhone = phone,
                MobilePhone = phone,
                MemberSince = DateTime.Now,
                LastUpdated = DateTime.Now

            };

            context.People.Add(userProfile);
            context.SaveChanges();
            userProfile.Children = children;
            context.SaveChanges();

            return userProfile;
        }


    }
}