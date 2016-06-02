using System;
using System.Collections.Concurrent;
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

        if (result.SpotifyAuthTokens != null 
          && (!result.SpotifyAuthTokens.ReceivedTime.HasValue
            ||
            (DateTime.UtcNow - result.SpotifyAuthTokens.ReceivedTime.Value).TotalSeconds > Math.Round(result.SpotifyAuthTokens.ExpiresIn/2.0) //update token if it halfway expired
            )
          )
        {
          var newTokens = SpotifyServices.RenewSpotifyTokens(result.SpotifyAuthTokens);
          result.SpotifyAuthTokens = newTokens;

        }
        result.ShouldAutoSave = true;

        return result;
      }
    }

    public static Session CurrentSession
    {
      get
      {
        string sessionId;
        var sessionCookie = HttpContext.Current.Request.Cookies[sessionCookieName];
        if (sessionCookie == null || string.IsNullOrWhiteSpace(sessionCookie.Value))
        {
          sessionCookie = HttpContext.Current.Response.Cookies[sessionCookieName];
        }
        if (sessionCookie != null && string.IsNullOrWhiteSpace(sessionCookie.Value))
        {
          sessionCookie = null;
        }

        if (sessionCookie == null)
        {
          sessionId = Guid.NewGuid().ToString("N");
          sessionCookie= new HttpCookie(sessionCookieName, sessionId);
        }
        else
        {
          sessionId = sessionCookie.Value;
        }
        sessionCookie.Expires = DateTime.Now.AddDays(30);
        HttpContext.Current.Response.Cookies.Add(sessionCookie);

        var session = GetSession(sessionId);
        if (session == null)
        {
          session = new Session{SessionId = sessionId};
        }

        return session;
      }
    }

    protected static ConcurrentDictionary<string,long> FakeGameTicks = new ConcurrentDictionary<string, long>();

    public static long GetGameTick(string gameId)
    {
      if (FakeGameTicks.ContainsKey(gameId))
        return FakeGameTicks[gameId];
      return 0;

      //return Db.GetGameUpdateTick(gameId);
    }
    public static void UpdateGameTick(Model.Game game)
    {
      long newTick = 0;
      if (game.BallotCreationTime.HasValue && game.MinimumVotesCastTime.HasValue)
      {
        newTick = (game.BallotCreationTime.Value > game.MinimumVotesCastTime.Value
          ? game.BallotCreationTime.Value
          : game.MinimumVotesCastTime.Value).Ticks;
      }
      else if (game.BallotCreationTime.HasValue)
      {
        newTick = game.BallotCreationTime.Value.Ticks;
      }
      else if(game.MinimumVotesCastTime.HasValue)
      {
        newTick = game.MinimumVotesCastTime.Value.Ticks;
      }

      FakeGameTicks[game.GameId] = newTick;
      return;
      //Db.SetGameUpdateTick(game.GameId,newTick);
    }
  }
}