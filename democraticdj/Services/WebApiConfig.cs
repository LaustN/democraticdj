using System.Net.Http.Formatting;
using System.Web.Http;

namespace Democraticdj.Services
{
  public static class WebApiConfig
  {
    public static void Register(HttpConfiguration config)
    {
      // Web API configuration and services

      // Web API routes
      config.Formatters.Clear();
      config.Formatters.Add(new JsonMediaTypeFormatter());
      config.MapHttpAttributeRoutes();
    }
  }
}
