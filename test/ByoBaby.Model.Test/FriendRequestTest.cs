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
    public class FriendRequestTests
    {
        #region init

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

        #endregion

        #region tests

        [TestMethod]
        public void FriendRequestTest_CreateTest()
        {
            long requestorId = SeededPeople[0].Id;
            long audienceId = SeededPeople[1].Id;

            using (var repo = new ByoBabyRepository())
            {
                Assert.IsNotNull(repo);

                var requestor = repo.People
                    .Include(u => u.Friends)
                    .FirstOrDefault(u => u.Id == requestorId);

                var audience = repo.People
                    .Include(u => u.Friends)
                    .Include(u => u.PendingRequests)
                    .FirstOrDefault(u => u.Id == audienceId);
                var fr = new FriendRequest()
                {
                    Title = "Wait a minute.... You have a kid too?",
                    Description = "Hi Guy! I'd like to hang-out, play-date and stuff.",
                    Requestor = requestor,
                    Audience = audience
                };
                audience.PendingRequests.Add(fr);

                repo.SaveChanges();
            }

            using (var repo = new ByoBabyRepository())
            {
                Assert.IsNotNull(repo);

                var audience = repo.People
                    .Include(u => u.Friends)
                    .Include(u => u.PendingRequests)
                    .FirstOrDefault(u => u.Id == audienceId);

                Assert.AreEqual(1, audience.PendingRequests.Count);
                Assert.AreEqual(1, repo.Requests.Count());
            }
        }

        [TestMethod]
        public void FriendRequestTest_AcceptTest()
        {
            long requestorId = SeededPeople[1].Id;
            long audienceId = SeededPeople[0].Id;
            long requestId;
            using (var repo = new ByoBabyRepository())
            {
                Assert.IsNotNull(repo);

                var requestor = repo.People
                    .Include(u => u.Friends)
                    .FirstOrDefault(u => u.Id == requestorId);

                var audience = repo.People
                    .Include(u => u.Friends)
                    .Include(u => u.PendingRequests)
                    .FirstOrDefault(u => u.Id == audienceId);
                var fr = new FriendRequest()
                {
                    Title = "Wait a minute.... You have a kid too?",
                    Description = "Hi Guy! I'd like to hang-out, play-date and stuff.",
                    Requestor = requestor,
                    Audience = audience
                };
                audience.PendingRequests.Add(fr);

                repo.SaveChanges();
                requestId = fr.Id;
            }

            using (var repo = new ByoBabyRepository())
            {
                Assert.IsNotNull(repo);

                var request = repo.Requests
                    .Include("Requestor")
                    .Include("Requestor.Friends")
                    .Include("Audience")
                    .Include("Audience.Friends")
                    .FirstOrDefault(u => u.Id == requestId);

                Assert.IsTrue(request.Audience != null);
                Assert.IsTrue(request.Requestor != null);

                request.Accept();
            }
            using (var repo = new ByoBabyRepository())
            {
                Assert.IsNotNull(repo);
                var request = repo.Requests
                   .Include("Requestor")
                   .Include("Requestor.Friends")
                   .Include("Audience")
                   .Include("Audience.Friends")
                   .FirstOrDefault(u => u.Id == requestId);

                Assert.IsNull(request);

                var requestor = repo.People
                   .Include(u => u.Friends)
                   .FirstOrDefault(u => u.Id == requestorId);

                var audience = repo.People
                    .Include(u => u.Friends)
                    .Include(u => u.PendingRequests)
                    .FirstOrDefault(u => u.Id == audienceId);

                Assert.IsTrue(requestor.Friends.Any(f => f.Id == audienceId));
                Assert.IsTrue(audience.Friends.Any(f => f.Id == requestorId));
            }

        }

        [TestMethod]
        public void FriendRequestTest_DenyTest()
        {
            long requestorId = SeededPeople[1].Id;
            long audienceId = SeededPeople[2].Id;
            long requestId;
            using (var repo = new ByoBabyRepository())
            {
                Assert.IsNotNull(repo);

                var requestor = repo.People
                    .Include(u => u.Friends)
                    .FirstOrDefault(u => u.Id == requestorId);

                var audience = repo.People
                    .Include(u => u.Friends)
                    .Include(u => u.PendingRequests)
                    .FirstOrDefault(u => u.Id == audienceId);
                var fr = new FriendRequest()
                {
                    Title = "Wait a minute.... You have a kid too?",
                    Description = "Hi Guy! I'd like to hang-out, play-date and stuff.",
                    Requestor = requestor,
                    Audience = audience
                };
                audience.PendingRequests.Add(fr);

                repo.SaveChanges();
                requestId = fr.Id;
            }

            using (var repo = new ByoBabyRepository())
            {
                Assert.IsNotNull(repo);

                var request = repo.Requests
                    .Include("Requestor")
                    .Include("Requestor.Friends")
                    .Include("Audience")
                    .Include("Audience.Friends")
                    .FirstOrDefault(u => u.Id == requestId);

                Assert.IsTrue(request.Audience != null);
                Assert.IsTrue(request.Requestor != null);

                request.Deny();
            }
            using (var repo = new ByoBabyRepository())
            {
                Assert.IsNotNull(repo);
                var request = repo.Requests
                   .Include("Requestor")
                   .Include("Requestor.Friends")
                   .Include("Audience")
                   .Include("Audience.Friends")
                   .FirstOrDefault(u => u.Id == requestId);

                Assert.IsNull(request);

                var requestor = repo.People
                   .Include(u => u.Friends)
                   .FirstOrDefault(u => u.Id == requestorId);

                var audience = repo.People
                    .Include(u => u.Friends)
                    .Include(u => u.PendingRequests)
                    .FirstOrDefault(u => u.Id == audienceId);

                Assert.IsFalse(requestor.Friends.Any(f => f.Id == audienceId));
                Assert.IsFalse(audience.Friends.Any(f => f.Id == requestorId));
            }

        }

        #endregion
    }
}
