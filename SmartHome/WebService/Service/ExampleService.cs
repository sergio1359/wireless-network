using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebService.Request;
using WebService.Response;

namespace WebService.Service
{
    /// <summary>
    /// Create your ServiceStack web service implementation.
    /// </summary>
    public class HelloService : IService
    {
        public object Any(HelloRequest request)
        {
            //Looks strange when the name is null so we replace with a generic name.
            var name = request.Name ?? "SmartHome";
            return new HelloResponse { Result = "Hello, " + name };
        }
    }
}