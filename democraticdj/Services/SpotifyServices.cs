using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Configuration;
using Democraticdj.Model;
using Democraticdj.Model.Spotify;
using Newtonsoft.Json;

namespace Democraticdj.Services
{
  public class SpotifyServices
  {
    public static string GetAuthUrl(string state)
    {
      string spotifyClientId = WebConfigurationManager.AppSettings[Constants.ConfigurationKeys.SpotifyClientId];

      // 0 = client id - as provided by https://developer.spotify.com/my-applications/
      // 1 = redirect URI - should likely be the local version of authcomplete.aspx
      // 2 = scope - see https://developer.spotify.com/web-api/using-scopes/
      // 3 = state - will be passed back, see that it matches in order to assert that callback is from correct origin

      var result = string.Format(
        Constants.SpotifyUrls.SpotifyAuthUrlTemplate,
        spotifyClientId,
        HttpUtility.UrlEncode(RedirectUrl),
        "playlist-modify-private playlist-read-private playlist-modify-public",
        HttpUtility.UrlEncode(state)
        );
      return result;
    }

    private static string Base64Encode(string plainText)
    {
      var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
      return System.Convert.ToBase64String(plainTextBytes);
    }

    public static SpotifyTokens ProcessAuthCode(string authCode, string state)
    {

      var client = GetClient();
      NameValueCollection values = new NameValueCollection();

      values.Add("grant_type", "authorization_code");
      values.Add("code", authCode);
      values.Add("redirect_uri", RedirectUrl);
      var idAndSecret = WebConfigurationManager.AppSettings[Constants.ConfigurationKeys.SpotifyClientId] + ":" +
                        WebConfigurationManager.AppSettings[Constants.ConfigurationKeys.SpotifyClientSecret];
      client.Headers.Add("Authorization", "Basic " + Base64Encode(idAndSecret));

      var result = client.UploadValues(new Uri("https://accounts.spotify.com/api/token"), values);
      string decoded = client.Encoding.GetString(result);
      var deserialized = Newtonsoft.Json.JsonConvert.DeserializeObject<SpotifyTokens>(decoded);

      return deserialized;

    }

    public static SpotifyTokens RenewSpotifyTokens(SpotifyTokens oldTokens)
    {
      var client = GetClient();
      NameValueCollection values = new NameValueCollection();

      values.Add("grant_type", "refresh_token");
      values.Add("refresh_token", oldTokens.RefreshToken);

      var idAndSecret = WebConfigurationManager.AppSettings[Constants.ConfigurationKeys.SpotifyClientId] + ":" +
                        WebConfigurationManager.AppSettings[Constants.ConfigurationKeys.SpotifyClientSecret];
      client.Headers.Add("Authorization", "Basic " + Base64Encode(idAndSecret));

      var result = client.UploadValues(new Uri("https://accounts.spotify.com/api/token"), values);
      string decoded = client.Encoding.GetString(result);
      var deserialized = Newtonsoft.Json.JsonConvert.DeserializeObject<SpotifyTokens>(decoded);

      deserialized.RefreshToken = oldTokens.RefreshToken;
      deserialized.ReceivedTime = DateTime.UtcNow;
      return deserialized;

    }

    public static string RedirectUrl
    {
      get
      {

        return (HttpContext.Current.Request.IsSecureConnection ? "https://" : "http://") + HttpContext.Current.Request.Url.Host + "/usermanagement.aspx";
      }
    }

    public static SpotifyGetTracksResponse GetTracks(Model.Game game, string[] trackIds)
    {
      var client = GetClient();
      if (trackIds == null || trackIds.Length == 0)
      {
        return null;
      }

      var trackidsJoined = string.Join(",", trackIds.Where(trackId => !string.IsNullOrWhiteSpace(trackId)));
      if (string.IsNullOrWhiteSpace(trackidsJoined))
      {
        return null;
      }

      var tracksUrl = string.Format(
        Constants.SpotifyUrls.SpotifyTracksUrl,
        trackidsJoined
        );
      var result = client.DownloadString(tracksUrl);
      var deserialized = Newtonsoft.Json.JsonConvert.DeserializeObject<SpotifyGetTracksResponse>(result);
      return deserialized;


    }

    public static SpotifySearchResponse SearchForTracks(Model.Game game, string query)
    {
      var client = GetClient();
      using (User user = StateManager.Db.GetUser(game.UserId))
      {
        client.Headers.Add("Authorization", "Bearer " + user.SpotifyAuthTokens.AccessToken);
      }


      var searchUrl = string.Format(
        Constants.SpotifyUrls.SpotifySearchUrl,
        HttpUtility.UrlEncode(query)
        );

      var result = client.DownloadString(searchUrl);
      var deserialized = Newtonsoft.Json.JsonConvert.DeserializeObject<SpotifySearchResponse>(result);
      return deserialized;

    }


    public static Playlist CreatePlayList(SpotifyTokens spotifyTokens, string userId, string listName)
    {
      var client = GetClient();
      client.Headers.Add("Authorization", "Bearer " + spotifyTokens.AccessToken);
      client.Headers.Add("Content-Type", "application/json");

      var request = new CreatePlaylistRequest
      {
        Name = listName,
        Public = true
      };
      var serializedRequest = JsonConvert.SerializeObject(request);
      var urlToCall = string.Format(Constants.SpotifyUrls.SpotifyUsersPlaylistsUrlWithPlaceholder, userId);

      var result = client.UploadString(urlToCall, serializedRequest);
      var deserialized = Newtonsoft.Json.JsonConvert.DeserializeObject<Playlist>(result);
      return deserialized;
    }

    public static SpotifyPlaylistsResponse GetPlaylists(SpotifyTokens spotifyTokens)
    {
      var client = GetClient();
      client.Headers.Add("Authorization", "Bearer " + spotifyTokens.AccessToken);

      var result = client.DownloadString(Constants.SpotifyUrls.SpotifyMyPlaylistsUrl);
      var deserialized = Newtonsoft.Json.JsonConvert.DeserializeObject<SpotifyPlaylistsResponse>(result);
      return deserialized;
    }

    public static IEnumerable<Track> GetTracksFromPlaylist(Playlist playlist, SpotifyTokens spotifyTokens)
    {
      if (playlist == null || playlist.Owner == null || playlist.Owner.Id == null || playlist.Id == null)
        yield break;

      var client = GetClient();
      client.Headers.Add("Authorization", "Bearer " + spotifyTokens.AccessToken);

      PlaylistTracksResponse deserialized = null;

      var url = string.Format(Constants.SpotifyUrls.SpotifyGetTracksFromPlaylistUrl, playlist.Owner.Id, playlist.Id);
      var result = client.DownloadString(url);
      deserialized = Newtonsoft.Json.JsonConvert.DeserializeObject<PlaylistTracksResponse>(result);


      if (deserialized != null)
      {
        foreach (var track in deserialized.Tracks)
        {
          yield return track.Track;
        }
      }
    }

    public static SpotifyUser GetAuthenticatedUser(SpotifyTokens spotifyTokens)
    {
      var client = GetClient();
      client.Headers.Add("Authorization", "Bearer " + spotifyTokens.AccessToken);

      var result = client.DownloadString(Constants.SpotifyUrls.SpotifyMeUrl);
      var deserialized = Newtonsoft.Json.JsonConvert.DeserializeObject<SpotifyUser>(result);
      return deserialized;
    }

    private static WebClient GetClient()
    {
      var client = new WebClient();
      client.Encoding = Encoding.UTF8;
      return client;
    }

    public static void AppendTrackToPlaylist(Model.Game game, string bestTrackId)
    {
      var client = GetClient();

      string spotifyUserId = null;
      using (User user = StateManager.Db.GetUser(game.UserId))
      {
        client.Headers.Add("Authorization", "Bearer " + user.SpotifyAuthTokens.AccessToken);
        spotifyUserId = user.SpotifyUser.Id;

      }

      var addTrackToPlaylistUrl = string.Format(
        Constants.SpotifyUrls.SpotifyAddToPlaylistUrl,
        spotifyUserId,
        game.SpotifyPlaylistId,
        bestTrackId
        );

      var result = client.UploadData(new Uri(addTrackToPlaylistUrl), new byte[0]);
      var stringifiedResult = client.Encoding.GetString(result);
    }

    public static void ReOrderPlaylist(Model.Game game, string[] trackIds)
    {
      var client = GetClient();

      string spotifyUserId = null;
      using (User user = StateManager.Db.GetUser(game.UserId))
      {
        client.Headers.Add("Authorization", "Bearer " + user.SpotifyAuthTokens.AccessToken);
        spotifyUserId = user.SpotifyUser.Id;
      }

      var addTrackToPlaylistUrl = string.Format(
        Constants.SpotifyUrls.SpotifyReorderPlaylistUrl,
        spotifyUserId,
        game.SpotifyPlaylistId,
        string.Join(",", trackIds.Where(id => !string.IsNullOrWhiteSpace(id)).Select(id => "spotify:track:" + id))
        );

      var result = client.UploadData(new Uri(addTrackToPlaylistUrl), "PUT", new byte[0]);
      var stringifiedResult = client.Encoding.GetString(result);
    }
  }
}