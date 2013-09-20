using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Xml;
using System.Xml.Linq;
using ByoBaby.Rest.Models;
using ByoBaby.Model;
using ByoBaby.Model.Repositories;
using ByoBaby.Security;
using YelpSharp;
using YelpSharp.Data;
using YelpSharp.Data.Options;

namespace ByoBaby.Rest.Controllers
{
    public class NearByController : ApiController
    {
        #region consts

        //TODO - move to web.config
        private const string BingMapsKey = "AuBiDC9YFcYJr09uJZnkeKb_bflX5EbLUNU7wVJ7E0P414Ptj4kIMq3GqLOQQEB6";
        private const string YelpV2ConsumerKey = "urh0CQQyRtrG7Li6ro-faA";
        private const string YelpV2ConsumerSecret = "o1Yb0CipArdBmqhCs1Fq_OGVSMM";
        private const string YelpV2Token = "2CJWkwJFmh1vksIVoVL9XtAQZv7VSNZM";
        private const string YelpV2TokenSecret = "ZjVjpt2A_3dhNEC4GmB-UyfWZMs";
        private const string YelpV2SignatureMethod = "HMAC-SHA1";
        private static readonly Uri YelpURLBase = new Uri("http://api.yelp.com/v2/");
        private static readonly Uri GeoCodeURLBase = new Uri("http://dev.virtualearth.net/REST/v1/Locations");
        private const string GeoCodeXmlNamespace = "http://schemas.microsoft.com/search/local/ws/rest/v1";
        #endregion

        #region members

        private static readonly Options yelpOptions = new Options()
        {
            AccessToken = YelpV2Token,
            AccessTokenSecret = YelpV2TokenSecret,
            ConsumerKey = YelpV2ConsumerKey,
            ConsumerSecret = YelpV2ConsumerSecret
        };

        private ByoBabyRepository db = new ByoBabyRepository();

        #endregion

        #region api controller actions

        [Authorize()]
        public IEnumerable<LocationModel> GetLocations(double lat, double lon)
        {
            List<LocationModel> locations = new List<LocationModel>();
            var y = new Yelp(yelpOptions);
            var searchOptions = new SearchOptions()
            {
                GeneralOptions = new GeneralOptions() { radius_filter = 5000 },
                LocationOptions = new CoordinateOptions() { latitude = lat, longitude = lon }
            };

            var task = y.Search(searchOptions).ContinueWith((results) => {
                foreach (var business in results.Result.businesses.OrderBy(b => b.distance))
                {
                    StringBuilder builder = new StringBuilder(business.location.address[0]);
                    for(int i = 1; i < business.location.address.Length; i++)
                    {
                        builder.AppendFormat(" {0}", business.location.address[i]);
                    }
                    builder.AppendFormat(" {0}", business.location.city);
                    builder.AppendFormat(" {0}", business.location.state_code);
                    builder.AppendFormat(" {0}", business.location.postal_code);

                    var loc = new LocationModel()
                    {
                        YelpId = business.id,
                        ImageUrl = business.image_url,
                        PhoneNumber = business.phone,
                        Name = business.name,
                        Address = builder.ToString()
                    };

                    GeocodeAddress(loc);
                    locations.Add(loc);
                }
            });

            Task.WaitAll(new Task[] { task });

            return locations;
        }

        [Authorize()]
        public IEnumerable<object> GetPeople(double lat, double lon)
        {
            throw new NotImplementedException();
        }

        [Authorize()]
        public HttpResponseMessage CheckIn([FromBody] CheckInModel checkin)
        {
             ByoBabiesUserPrincipal currentUser =
                    HttpContext.Current.User as ByoBabiesUserPrincipal;

            var id = currentUser.GetPersonId();
            if (ModelState.IsValid)
            {
                Person person = db.People.FirstOrDefault(u => u.Id == id);
                
                var ci = new CheckIn()
                {
                    Owner = person,
                    Duration = checkin.Duration,
                    LocationId = checkin.Location.YelpId,
                    Latitude = checkin.Location.Latitude.Value,
                    Longitude = checkin.Location.Longitude.Value,
                    Note = checkin.Note,
                    StartTime = DateTime.Now
                };

                db.CheckIns.Add(ci);

                db.SaveChanges();

                return Request.CreateResponse(HttpStatusCode.OK, ci.Id);
            }
            else
            {
                throw new HttpResponseException(
                         new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest)
                         {
                             ReasonPhrase = "Check-in failed, invalid information."
                         });
            }
        }

        [Authorize()]
        public HttpResponseMessage CheckOut(long id)
        {
            ByoBabiesUserPrincipal currentUser =
                   HttpContext.Current.User as ByoBabiesUserPrincipal;

            var userId = currentUser.GetPersonId();
            var checkin = db.CheckIns.Include(c => c.Owner).FirstOrDefault(c => c.Id == id);
            if (checkin != null)
            {
                if (checkin.Owner.Id == userId)
                {
                    db.CheckIns.Remove(checkin);
                    db.SaveChanges();
                }
                else
                {
                    throw new HttpResponseException(HttpStatusCode.Unauthorized);
                }
            }
            else
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return new HttpResponseMessage(HttpStatusCode.OK);
        }


        #endregion

        #region methods

        private void GeocodeAddress(LocationModel model)
        {
            UriBuilder b = new UriBuilder(GeoCodeURLBase);
            b.Query = string.Format(
                System.Globalization.CultureInfo.InvariantCulture,
                "key={0}&o=xml&query={1}",
                BingMapsKey,
                System.Web.HttpUtility.UrlEncode(model.Address));


            HttpWebRequest request = CreateWebRequest(b.ToString());

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                var responseValue = string.Empty;

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    string message = String.Format("POST failed. Received HTTP {0}", response.StatusCode);
                    throw new ApplicationException(message);
                }

                // grab the response  
                using (var responseStream = response.GetResponseStream())
                {
                    using (var reader = new StreamReader(responseStream))
                    {
                        responseValue = reader.ReadToEnd();
                    }
                }

                if (!string.IsNullOrEmpty(responseValue))
                {
                    var doc = XDocument.Parse(responseValue);

                    var location = doc.Descendants(XName.Get("Location", GeoCodeXmlNamespace)).FirstOrDefault();
                    if (location != null)
                    {
                        var point = location.Descendants(XName.Get("Point", GeoCodeXmlNamespace)).FirstOrDefault();
                        if (point != null)
                        {
                            model.Latitude = Double.Parse(point.Descendants(XName.Get("Latitude", GeoCodeXmlNamespace)).First().Value);
                            model.Longitude = Double.Parse(point.Descendants(XName.Get("Longitude", GeoCodeXmlNamespace)).First().Value);
                        }
                    }
                }
            }
        }  

        private HttpWebRequest CreateWebRequest(string endPoint)
        {
            var request = (HttpWebRequest)WebRequest.Create(endPoint);

            request.Method = "GET";
            request.ContentLength = 0;
            request.ContentType = "text/xml";

            return request;
        }
    
        #endregion

    }
}
