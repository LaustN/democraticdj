using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Democraticdj.Services;
using Newtonsoft.Json;

namespace Democraticdj.Model
{
  public class Game
  {
    public string SpotifyUserId;
    public string SpotifyUserName;
    public string SpotifyPlaylistId;


  }

  public class Player
  {
    public string PlayerId;

    public List<string> SelectedTracks; //just a set of spotify track IDs

    public int Points;

  }

  public class User : IDisposable
  {
    public MongoDB.Bson.ObjectId Id { get; set; }
    public string UserId { get; set; }
    public string DisplayName { get; set; }
    public string Email { get; set; }
    public string EmailIsVerified { get; set; }
    public Spotify.SpotifyTokens SpotifyAuthTokens { get; set; }
    public void Dispose()
    {
      StateManager.SetUser(this);
    }
  }

  public class Session :IDisposable
  {
    public MongoDB.Bson.ObjectId Id { get; set; }
    public string SessionId { get; set; }
    public string GameId { get; set; }
    public string UserId { get; set; }

    public void Dispose()
    {
      StateManager.SetSession(this);
    }
  }
}