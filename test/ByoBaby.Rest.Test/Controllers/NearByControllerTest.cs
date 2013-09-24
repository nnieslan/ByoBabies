using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Diagnostics;
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
using ByoBaby.Rest.Models;
namespace ByoBaby.Rest.Test.Controllers
{
    [TestClass]
    public class NearByControllerTest
    {
        private static NearByController InitializeController(HttpMethod verb, string url)
        {
            var values = new HttpRouteValueDictionary() { 
                    { "controller", "locations" } 
                };
            return ControllerTestHelper.InitializeController<NearByController>(
                verb, url,
                "application",
                "DefaultApi",
                "api/{controller}/{id}",
                values);
        }

        [TestMethod]
        public void NearByController_ConstructorTest()
        {
            var controller = new NearByController();
            Assert.IsNotNull(controller);
        }

        [TestMethod]
        public void NearByController_GetLocationsTest()
        {
            double lat = 39.7561387;
            double lon = -104.9272044;
            var controller = InitializeController(HttpMethod.Get,
               "http://localhost/byobabies/api/locations");

            var locations = controller.GetLocations(lat, lon);

            Assert.IsNotNull(locations);
            Assert.IsTrue(locations.Count() > 0);

            foreach (var l in locations)
            {
                Assert.IsTrue(l.Latitude.HasValue);
                Assert.IsTrue(l.Longitude.HasValue);
            }

        }

        //[TestMethod]
        //public void NearByController_GetPeopleTest()
        //{
        //    double lat = 39.7561387;
        //    double lon = -104.9272044;

        //    var checkins = new ByoBabyRepository().CheckIns.Include(c => c.Owner)
        //        .Select(c => new { Checkin = c, Distance = NearByController.CalcHaversineDistance(lat, c.Latitude, lon, c.Longitude) })
        //        .Where(c => c.Distance <= 4000)
        //        .OrderBy(c => c.Distance);

        //    Assert.IsNotNull(checkins);
        //    Assert.IsTrue(checkins.Count() > 0);

        //    foreach (var l in checkins)
        //    {
        //        Debug.WriteLine("{0} - {1}", l.Distance, l.Checkin.Owner.Email);
               
        //    }

        //}

        [TestMethod]
        public void NearByController_2357Eudora_GetTest()
        {
            double lat = 39.752222;
            double lon = -104.930832;
            var controller = InitializeController(HttpMethod.Get,
               "http://localhost/byobabies/api/locations");

            var locations = controller.GetLocations(lat, lon);

            Assert.IsNotNull(locations);
            Assert.IsTrue(locations.Count() > 0);

            foreach (var l in locations)
            {
                if (l.Latitude.HasValue && l.Longitude.HasValue)
                {
                    Debug.WriteLine(string.Format("{0} - Lat: {1} | Lon: {2}", l.YelpId, l.Latitude, l.Longitude));
                }
            }

        }

    }
}
