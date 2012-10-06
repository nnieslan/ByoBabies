using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ByoBaby.Model;
using ByoBaby.Model.Repositories;

namespace ByoBaby.Model.Test
{
    [TestClass]
    public class ByoBabyRepositoryTest
    {
        [TestMethod]
        public void GetProfilesTest()
        {
            var repo = new ByoBabyRepository();

            var profiles = (from p in repo.People select p);

            Assert.IsNotNull(profiles);
        }
    }
}
