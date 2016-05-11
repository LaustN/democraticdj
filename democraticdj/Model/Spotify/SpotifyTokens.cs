using System;
using Newtonsoft.Json;

namespace Democraticdj.Model.Spotify
{
  public class SpotifyTokens
  {
    [JsonProperty("access_token")]
    public string AccessToken { get; set; }

    [JsonProperty("token_type")]
    public string TokenType { get; set; }

    [JsonProperty("expires_in")]
    public int ExpiresIn { get; set; }

    [JsonProperty("refresh_token")]
    public string RefreshToken { get; set; }

    [JsonProperty("receivedOn")]
    public DateTime? ReceivedTime { get; set; }
  }
}