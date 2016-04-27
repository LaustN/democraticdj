using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Web;
using System.Web.Configuration;
using Democraticdj.Model;

namespace Democraticdj.Services
{
  public class SpotifyAuthProvider
  {
    public static string GetAuthUrl(string state)
    {
      string spotifyClientId = WebConfigurationManager.AppSettings[Constants.ConfigurationKeys.SpotifyClientId];

      // 0 = client id - as provided by https://developer.spotify.com/my-applications/
      // 1 = redirect URI - should likely be the local version of authcomplete.aspx
      // 2 = scope - see https://developer.spotify.com/web-api/using-scopes/
      // 3 = state - will be passed back, see that it matches in order to assert that callback is from correct origin

      var result = string.Format(
        Constants.Authentication.SpotifyAuthUrlTemplate, 
        spotifyClientId,
        HttpUtility.UrlEncode(RedirectUrl),
        "playlist-modify-public",
        HttpUtility.UrlEncode(state)
        );
      return result;
    }

    public static SpotifyTokens ProcessAuthCode(string authCode, string state)
    {

      var client = new WebClient();
      NameValueCollection values = new NameValueCollection();

      values.Add("grant_type", "authorization_code");
      values.Add("code", authCode);
      values.Add("redirect_uri", RedirectUrl);
      values.Add("client_id", WebConfigurationManager.AppSettings[Constants.ConfigurationKeys.SpotifyClientId]);
      values.Add("client_secret", WebConfigurationManager.AppSettings[Constants.ConfigurationKeys.SpotifyClientSecret]);

      var result = client.UploadValues(new Uri("https://accounts.spotify.com/api/token"), values);
      string decoded = client.Encoding.GetString(result);
      var deserialized = Newtonsoft.Json.JsonConvert.DeserializeObject<SpotifyTokens>(decoded);

      return deserialized;
    }

    public static string RedirectUrl
    {
      get
      {
        return "http://" + HttpContext.Current.Request.Url.Host + "/authcompleted.aspx";
      }
    }
  }
}