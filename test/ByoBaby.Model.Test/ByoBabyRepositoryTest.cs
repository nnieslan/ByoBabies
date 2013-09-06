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
    public class ByoBabyRepositoryTest
    {
        [ClassInitialize()]
        public static void Initialize(TestContext context)
        {
            using(var oldDb = new ByoBabyRepository()) {
                oldDb.Database.Delete();
            }
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
                Children = new Collection<Child>() { new Child() { Age = 1, Gender = "M" } }
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
        public void ByoBabyRepository_ConstructorTest()
        {
            using (var repo = new ByoBabyRepository())
            {
                Assert.IsNotNull(repo);
            }
        }

        [TestMethod]
        public void ByoBabyRepository_GetProfilesTest()
        {
            using (var repo = new ByoBabyRepository())
            {

                var profiles = (from p in repo.People select p);

                Assert.IsNotNull(profiles);
                Assert.AreEqual(SeededPeople.Count, profiles.Count());
            }
        }

    }
}
