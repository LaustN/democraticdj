using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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

  public class User
  {
    public string DisplayName { get; set; }
    public string Email { get; set; }
    public string EmailIsVerified { get; set; }
    public Spotify.SpotifyTokens SpotifyAuthTokens { get; set; }
  }

  public class Session
  {
    public MongoDB.Bson.ObjectId Id { get; set; }
    public string SessionId { get; set; }
    public string GameId { get; set; }
    public string UserId { get; set; }
  }
}