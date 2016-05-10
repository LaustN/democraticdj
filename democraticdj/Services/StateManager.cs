﻿using System;
using System.Collections.Specialized;
using System.Net;
using System.Web;
using System.Web.Configuration;
using Democraticdj.Model;

namespace Democraticdj.Services
{
  public class StateManager
  {
    private const string sessionCookieName = "user";

    private static DbConnector _db;
    public static DbConnector Db
    {
      get { return _db ?? (_db = new DbConnector()); }
    }

    protected Session CreateSession()
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

    public static void SetUser(User user)
    {
      Db.SaveUser(user);
    }

    public static User CurrentUser
    {
      get
      {
        User result = null;
        using (Session session = CurrentSession)
        {
          if (!string.IsNullOrWhiteSpace(session.UserId))
          {
            result = Db.GetUser(session.UserId);
          }
          if (result == null)
          {
            result = new User { UserId = Guid.NewGuid().ToString("N") };
            session.UserId = result.UserId;
          }
        }

        return result;
      }
    }

    public static Session CurrentSession
    {
      get
      {
        string sessionId;
        var sessionCookie = HttpContext.Current.Request.Cookies[sessionCookieName] ?? HttpContext.Current.Response.Cookies[sessionCookieName];
        if (sessionCookie == null)
        {
          sessionId = Guid.NewGuid().ToString("N");
          sessionCookie= new HttpCookie(sessionCookieName, sessionId);
        }
        else
        {
          sessionId = sessionCookie.Value;
        }
        sessionCookie.Expires = DateTime.Now.AddMinutes(20.0);
        HttpContext.Current.Response.Cookies.Add(sessionCookie);

        var session = GetSession(sessionId);
        if (session == null)
        {
          session = new Session{SessionId = sessionId};
        }

        return session;
      }
    }

  }
}