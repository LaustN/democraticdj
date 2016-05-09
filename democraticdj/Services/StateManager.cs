using System;
using System.Collections.Specialized;
using System.Net;
using System.Web.Configuration;
using Democraticdj.Model;

namespace Democraticdj.Services
{
  public static class StateManager
  {

    private static DbConnector _db;
    public static DbConnector Db
    {
      get { return _db ?? (_db = new DbConnector()); }
    }

    public static Session CreateSession()
    {
      return new Session()
      {
        SessionId = Guid.NewGuid().ToString("N")
      };
    }

    public static Session GetSession(string sessionId)
    {
      return Db.GetSession(sessionId);
    }

    public static void SetSession(Session session)
    {
      Db.SaveSession(session);
    }

  }
}