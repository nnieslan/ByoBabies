using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ByoBaby.Model;
using ByoBaby.Model.Repositories;

namespace ByoBaby.Model.Test
{
    [TestClass]
    public class FriendsCrudTests
    {
        [ClassInitialize()]
        public static void Initialize(TestContext context)
        {
            Database.SetInitializer<ByoBabyRepository>(
                new Test_ByoBabyRepositoryInitializer());

            var db = new ByoBabyRepository();

            Person nickProfile = null;
            Person tiffanyProfile = null;
            Person willProfile = null;

            nickProfile = new Person()
            {
                UserId = Guid.NewGuid(),
                City = "Denver",
                State = "CO",
                Email = "nicknieslanik@gmail.com",
                ProfilePictureUrl = "http://m.c.lnkd.licdn.com/mpr/mpr/shrink_200_200/p/3/000/029/338/0961cc9.jpg",
                FirstName = "Nick",
                LastName = "Nieslanik",
                Neighborhood = "Park Hill",
                HomePhone = "720-939-9808",
                MobilePhone = "720-939-9808",
                MemberSince = DateTime.Now,
                LastUpdated = DateTime.Now

            };

            db.People.Add(nickProfile);
            db.SaveChanges();
            nickProfile.Children = new Collection<Child>()
                    {
                        new Child() { Parent = nickProfile, Name="Ephraim", Age=1, Gender = "M"} 
                    };
            db.SaveChanges();

            willProfile = new Person()
            {
                UserId = Guid.NewGuid(),
                City = "Denver",
                State = "CO",
                Email = "w.simpson@hotmail.com",
                FirstName = "Will",
                LastName = "Simpson",
                Neighborhood = "North Park Hill",
                HomePhone = "720-884-7684",
                MobilePhone = "720-884-7684",
                MemberSince = DateTime.Now,
                LastUpdated = DateTime.Now

            };
            db.People.Add(willProfile);
            db.SaveChanges();
            willProfile.Children = new Collection<Child>()
                    {
                        new Child() { Parent = willProfile, Name="Jude", Age=1, Gender = "M"} 
                    };
            db.SaveChanges();

            //if (willProfile != null && nickProfile != null)
            //{

            //    var fr = new FriendRequest()
            //    {
            //        Title = "Wait a minute.... You have a kid too?",
            //        Description = "Hi Guy! I'd like to hang-out, play-date and stuff.",
            //        Requestor = willProfile,
            //        Audience = nickProfile
            //    };

            //    nickProfile.PendingRequests = new Collection<Request>() { fr };
            //    nickProfile.Notifications = new Collection<Notification>() { new Notification() { Originator = fr } };

            //    db.SaveChanges();
            //}


            tiffanyProfile = new Person()
            {
                UserId = Guid.NewGuid(),
                FirstName = "Tiffany",
                LastName = "Nieslanik",
                Email = "tiffanynieslanik@gmail.com",
                MobilePhone = "303-819-4661",
                City = "Denver",
                Neighborhood = "North Park Hill",
                MemberSince = DateTime.Now.AddDays(-1),
                LastUpdated = DateTime.Now,
                Children = new Collection<Child>() { new Child() { Age = 1, Gender = "M", Name = "Ephraim" } }
            };

            db.People.Add(tiffanyProfile);
            db.SaveChanges();

            if (SeededPeople == null) { SeededPeople = new Collection<Person>(); }
            SeededPeople.Add(tiffanyProfile);
            SeededPeople.Add(nickProfile);
            SeededPeople.Add(willProfile);

        }

        private static Collection<Person> SeededPeople { get; set; }

        [TestMethod]
        public void FriendsCrudTests_AddFriendsTest()
        {
            long userId = SeededPeople[0].Id; 
            Collection<long> friendIds = new Collection<long>();
            using (var repo = new ByoBabyRepository())
            {
                Assert.IsNotNull(repo);

                var user = repo.People
                    .Include(u => u.Friends)
                    .FirstOrDefault(u => u.Id == userId);
                if (user.Friends == null) { user.Friends = new Collection<Person>(); }
                foreach (var other in repo.People.Where(u => u.Id != user.Id))
                {
                    user.Friends.Add(other);
                    friendIds.Add(other.Id);
                    repo.SaveChanges();
                }
            }

            using (var repo = new ByoBabyRepository())
            {
                var user = repo.People.Include(u => u.Friends).FirstOrDefault(u => u.Id == userId);

                Assert.IsNotNull(user);
                Assert.IsNotNull(user.Friends);
                Assert.AreEqual(friendIds.Count, user.Friends.Count);
                foreach (var id in friendIds)
                {
                    Assert.AreEqual(true, user.Friends.Any(f => f.Id == id));
                }
            }
        }

        [TestMethod]
        public void FriendsCrudTests_RemoveFriendTest()
        {
            long userId = SeededPeople[1].Id;
            Collection<long> friendIds = new Collection<long>();
            using (var repo = new ByoBabyRepository())
            {
                Assert.IsNotNull(repo);

                var user = repo.People.Include(u => u.Friends).FirstOrDefault(u => u.Id == userId);
                if (user.Friends == null) { user.Friends = new Collection<Person>(); }
                foreach (var other in repo.People.Where(u => u.Id != user.Id))
                {
                    user.Friends.Add(other);
                    friendIds.Add(other.Id);
                    repo.SaveChanges();
                }
            }

            using (var repo = new ByoBabyRepository())
            {
                var user = repo.People.Include(u => u.Friends).FirstOrDefault(u => u.Id == userId);

                Assert.IsNotNull(user);
                Assert.IsNotNull(user.Friends);
                Assert.AreEqual(friendIds.Count, user.Friends.Count);
                foreach (var friend in user.Friends.ToList())
                {
                    user.Friends.Remove(friend);
                }
                repo.SaveChanges();
            }
            using (var repo = new ByoBabyRepository())
            {
                var user = repo.People.Include(u => u.Friends).FirstOrDefault(u => u.Id == userId);

                Assert.IsNotNull(user);
                Assert.IsTrue(user.Friends == null || user.Friends.Count == 0);
            }
        }
    }
}
