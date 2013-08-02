using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Web;
using ByoBaby.Model;
using ByoBaby.Model.Repositories;

namespace ByoBaby.Rest.Test
{
    public class Test_ByoBabyRepositoryInitializer : DropCreateDatabaseAlways<ByoBabyRepository>
    {
        protected override void Seed(ByoBabyRepository context)
        {
            //TODO - seed the test data for unit tests.
        }
    }
}
