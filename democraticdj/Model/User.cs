using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using Democraticdj.Services;
using Newtonsoft.Json;

namespace Democraticdj.Model
{
  public class User : IDisposable
  {
    private List<UserEmail> _emails;
    public MongoDB.Bson.ObjectId Id { get; set; }
    public string UserId { get; set; }
    public string DisplayName { get; set; }
    public string AvatarUrl { get; set; }

    public List<UserEmail> Emails
    {
      get { return _emails ?? (_emails = new List<UserEmail>()); }
      set { _emails = value; }
    }

    public string UserName { get; set; }
    public string Password { get; set; }
    public Spotify.SpotifyTokens SpotifyAuthTokens { get; set; }
    public Spotify.SpotifyUser SpotifyUser { get; set; }

    [JsonIgnore]
    public bool ShouldAutoSave { get; set; }

    public void Dispose()
    {
      if (ShouldAutoSave)
      {
        StateManager.SetUser(this);
      }
    }

    [JsonIgnore]
    public bool IsLoggedIn
    {
      get
      {
        return SpotifyAuthTokens != null
              || Emails.Any(email => email.IsVerified)
              || !(
                  string.IsNullOrWhiteSpace(UserName)
                  ||
                  string.IsNullOrWhiteSpace(Password)
                  );

      }
    }
  }
}