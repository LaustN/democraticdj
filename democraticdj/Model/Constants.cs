using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Democraticdj.Model
{
  public class Constants
  {

    public struct SpotifyUrls
    {
      /// <summary>
      /// placeholders in SpotifyAuthUrlTemplate :
      /// 0 = client id - as provided by https://developer.spotify.com/my-applications/
      /// 1 = redirect URI - should likely be the local version of authcomplete.aspx
      /// 2 = scope - see https://developer.spotify.com/web-api/using-scopes/
      /// 3 = state - will be passed back, see that it matches in order to assert that callback is from correct origin
      /// </summary>
      public const string SpotifyAuthUrlTemplate = "https://accounts.spotify.com/authorize/?client_id={0}&response_type=code&redirect_uri={1}&scope={2}&state={3}";

      public const string SpotifyMyPlaylistsUrl = "https://api.spotify.com/v1/me/playlists";
      
      /// <summary>
      /// placeholders in SpotifyGetTracksFromPlaylistUrl :
      /// 0 = user_id
      /// 1 = playlist_id
      /// </summary>
      public const string SpotifyGetTracksFromPlaylistUrl = "https://api.spotify.com/v1/users/{0}/playlists/{1}/tracks";

      public const string SpotifyUsersPlaylistsUrlWithPlaceholder = "https://api.spotify.com/v1/users/{0}/playlists";
      public const string SpotifyMeUrl = "https://api.spotify.com/v1/me";
      public const string SpotifySearchUrl = "https://api.spotify.com/v1/search?type=track&q={0}";
      public const string SpotifyTracksUrl = "https://api.spotify.com/v1/tracks?ids={0}";


      /// <summary>
      /// placeholders in SpotifyAuthUrlTemplate :
      /// 0 = the spotify user id
      /// 1 = the playlist id
      /// 2 = the track id
      /// </summary>
      public const string SpotifyAddToPlaylistUrl = "https://api.spotify.com/v1/users/{0}/playlists/{1}/tracks?uris=spotify:track:{2}";
      
      public const string SpotifyReorderPlaylistUrl = "https://api.spotify.com/v1/users/{0}/playlists/{1}/tracks?uris={2}";
    }

    public struct ConfigurationKeys
    {
      public const string SpotifyClientId = "spotifyclientid";
      public const string SpotifyClientSecret = "spotifyclientsecret";
    }
  }
}