using System;
using Democraticdj.Services;

namespace Democraticdj.Model
{
  public class User : IDisposable
  {
    public MongoDB.Bson.ObjectId Id { get; set; }
    public string UserId { get; set; }
    public string DisplayName { get; set; }
    public string Email { get; set; }
    public string EmailIsVerified { get; set; }
    public string Password { get; set; }
    public Spotify.SpotifyTokens SpotifyAuthTokens { get; set; }
    public Spotify.SpotifyUser SpotifyUser { get; set; }
    public void Dispose()
    {
      StateManager.SetUser(this);
    }
  }
}