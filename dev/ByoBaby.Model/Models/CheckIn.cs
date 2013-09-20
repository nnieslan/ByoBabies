using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ServiceModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ByoBaby.Model.Repositories;


namespace ByoBaby.Model
{
    public class CheckIn
    {
        [Key]
        public long Id { get; set; }
        
        public Person Owner { get; set; }
        
        public int Duration { get; set; }
        
        public string Note { get; set; }

        public DateTime StartTime { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string LocationId { get; set; }
        

    }
}
