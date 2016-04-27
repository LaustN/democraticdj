using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Remoting.Contexts;
using System.Security.Cryptography;
using System.Threading;
using System.Web.Http;

namespace Democraticdj.Services
{

  [RoutePrefix("api")]
  public class RestServicesController : ApiController
  {
    private static bool _flag = false;

    [HttpGet]
    [Route("")]
    public string Get()
    {
      DateTime start = DateTime.Now;
      while (!_flag)
      {
        Thread.Sleep(20);
      }
      _flag = false;
      DateTime stop = DateTime.Now;
      return "gotten " + (stop-start);
    }

    [HttpPost]
    [Route("")]
    //public string Post([FromBody]string value)
    public string Post()
    {
      _flag = true;
      return "posted";
    }
  }
}