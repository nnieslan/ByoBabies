using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net.Http.Headers;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using ByoBaby.Rest.Models;
using ByoBaby.Model;
using ByoBaby.Model.Repositories;
using ByoBaby.Security;

namespace ByoBaby.Rest.Helpers
{
    public class CustomMultipartFormDataStreamProvider : MultipartFormDataStreamProvider
    {
        public CustomMultipartFormDataStreamProvider(string path)
            : base(path)
        { }

        public override string GetLocalFileName(HttpContentHeaders headers)
        {
            //TODO - prepend the user's profile id to the file name and normalize it.

            var name = !string.IsNullOrWhiteSpace(headers.ContentDisposition.FileName) 
                ? headers.ContentDisposition.FileName 
                : "NoName"; //TODO - generate a name
            
            return name.Replace("\"", string.Empty); //this is here because Chrome submits files in quotation marks which get treated as part of the filename and get escaped
        }
    }
}