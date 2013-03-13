using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Routing;
using System.Web.Http.Controllers;
using System.Web.Http.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ByoBaby.Rest.Controllers;
using ByoBaby.Model;
using ByoBaby.Model.Repositories;

namespace ByoBaby.Rest.Test.Controllers
{
    [TestClass]
    public class ProfileControllerTest
    {

        private static ProfileController InitializeController(HttpMethod verb, string url)
        {
            var values = new HttpRouteValueDictionary() { 
                    { "controller", "profile" } 
                };
            return ControllerTestHelper.InitializeController<ProfileController>(
                verb, url, 
                "application",
                "DefaultApi",
                "api/{user}/{controller}/{id}",
                values);
        }

        private static Person SeededPerson { get; set; }

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
                Children = new Collection<Child>() {new Child() { Age = 1, Gender = "M"} }
            };

            db.People.Add(sampleProfile);
            db.SaveChanges();

            SeededPerson = sampleProfile;
        }


        [TestMethod]
        public void ProfileController_ConstructorTest()
        {
            var controller = new ProfileController();
            Assert.IsNotNull(controller);
        }

        [TestMethod]
        public void ProfileController_GetTest()
        {
            var controller = InitializeController(HttpMethod.Get,
               "http://localhost/byobabies/api/profile");

            var profiles = controller.GetProfiles();

            Assert.IsNotNull(profiles);
            Assert.IsTrue(profiles.Count() > 0);

        }

        [TestMethod]
        public void ProfileController_GetSpecificTest()
        {
            var controller = InitializeController(HttpMethod.Get,
               "http://localhost/byobabies/api/profile");

            var profile = controller.GetProfile(SeededPerson.Id);

            Assert.IsNotNull(profile);
            Assert.AreEqual(SeededPerson.FirstName, profile.FirstName);
            Assert.AreEqual(SeededPerson.LastName, profile.LastName);
        }

        [TestMethod]
        public void ProfileController_PutTest()
        {
            var controller = InitializeController(HttpMethod.Get,
               "http://localhost/byobabies/api/profile");

            var oldPhone = SeededPerson.HomePhone;
            SeededPerson.HomePhone = "720-939-9808";

            var response = controller.PutProfile(SeededPerson.Id, SeededPerson);

            Assert.IsNotNull(response);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var savedProfile = controller.GetProfile(SeededPerson.Id);
            Assert.AreNotEqual(oldPhone, savedProfile.HomePhone);
            Assert.AreEqual("720-939-9808", savedProfile.HomePhone);

        }


    }
}
