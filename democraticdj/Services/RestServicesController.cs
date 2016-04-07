using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Remoting.Contexts;
using System.Security.Cryptography;
using System.Web.Http;

namespace Democraticdj.Services
{

  [RoutePrefix("api")]
  public class RestServicesController : ApiController
  {
    [HttpGet]
    [Route("")]
    public IEnumerable<string> Get()
    {
      return new string[] { "value1", "value2" };
    }

    [HttpGet]
    [Route("{id}")]
    public string Get(int id)
    {
      return "value";
    }

    [HttpPost]
    [Route("")]
    public void Post([FromBody]string value)
    {
    }

    [HttpPut]
    [Route("")]
    public void Put(int id, [FromBody]string value)
    {
    }

    [HttpDelete]
    [Route("")]
    public void Delete(int id)
    {
    }
  }
}