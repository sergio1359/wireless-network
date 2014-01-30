using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebService.Request
{
    /// <summary>
    /// Define your ServiceStack web service request (i.e. the Request DTO).
    /// </summary>    
    public class HelloRequest
    {
        public string Name { get; set; }
    }
}