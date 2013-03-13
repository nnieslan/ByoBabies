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
            Database.SetInitializer<ByoBabyRepository>(
                new Test_ByoBabyRepositoryInitializer());

            var db = new ByoBabyRepository();

            var sampleProfile = new Person()
            {
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

            db.People.Add(sampleProfile);
            db.SaveChanges();

            SeededPerson = sampleProfile;
        }

        private static Person SeededPerson { get; set; }

        [TestMethod]
        public void ByoBabyRepository_ConstructorTest()
        {
            var repo = new ByoBabyRepository();
            Assert.IsNotNull(repo);
        }

        [TestMethod]
        public void ByoBabyRepository_GetProfilesTest()
        {
            var repo = new ByoBabyRepository();

            var profiles = (from p in repo.People select p);

            Assert.IsNotNull(profiles);
            Assert.AreEqual(1, profiles.Count());
        }
    }
}
