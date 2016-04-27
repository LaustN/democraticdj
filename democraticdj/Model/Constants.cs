using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Democraticdj.Model
{
  public class Constants
  {

    public struct Authentication
    {
      /// <summary>
      /// placeholders in SpotifyAuthUrlTemplate :
      /// 0 = client id - as provided by https://developer.spotify.com/my-applications/
      /// 1 = redirect URI - should likely be the local version of authcomplete.aspx
      /// 2 = scope - see https://developer.spotify.com/web-api/using-scopes/
      /// 3 = state - will be passed back, see that it matches in order to assert that callback is from correct origin
      /// </summary>
      public const string SpotifyAuthUrlTemplate = "https://accounts.spotify.com/authorize/?client_id={0}&response_type=code&redirect_uri={1}&scope={2}&state={3}";
    }

    public struct ConfigurationKeys
    {
      public const string SpotifyClientId = "spotifyclientid";
      public const string SpotifyClientSecret = "spotifyclientsecret";
    }
  }
}