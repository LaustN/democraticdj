using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Democraticdj.Model;
using Democraticdj.Model.Spotify;
using Democraticdj.Services;

namespace Democraticdj
{
  public partial class CreateGame : System.Web.UI.Page
  {
    protected void Page_Load(object sender, EventArgs e)
    {
      string newGameId = null;
      bool newGameIdVerified = false;
      int idLength = 4;
      while (!newGameIdVerified)
      {
        newGameId = GetRandomString(idLength);
        var previousGame = StateManager.Db.GetGame(newGameId);
        if (previousGame == null)
        {
          newGameIdVerified = true;
        }
        else
        {
          idLength++;
        }
      }

      var createFromListId = Request.QueryString["createfromlistid"];
      var createFromListName = Request.Form["ListName"];
      if (!string.IsNullOrWhiteSpace(createFromListId) || !string.IsNullOrWhiteSpace(createFromListName))
      {
        using (var user = StateManager.CurrentUser)
        {
          Playlist playlist = null;

          if (!string.IsNullOrWhiteSpace(createFromListId))
          {
            playlist = SpotifyServices.GetPlaylists(user.SpotifyAuthTokens)
              .PlayLists.FirstOrDefault(playList => playList.Id == createFromListId);
          }

          if (!string.IsNullOrWhiteSpace(createFromListName))
          {
            playlist = SpotifyServices.CreatePlayList(user.SpotifyAuthTokens, user.SpotifyUser.Id, createFromListName);
          }

          if (playlist != null)
          {
            var game = new Model.Game
            {
              GameId = newGameId,
              SpotifyPlaylistId = createFromListId,
              SpotifyPlaylistUri = playlist.Uri,
              UserId = user.UserId,
              GameName = playlist.Name
            };

            foreach (var track in SpotifyServices.GetTracksFromPlaylist(playlist,user.SpotifyAuthTokens))
            {
              game.Nominees.Add(new Nominee{TrackId = track.Id,NominatingPlayerIds = new List<string>{user.UserId}});
            }
            StateManager.Db.SaveGame(game);
            Response.Redirect("/Game.aspx?gameid=" + game.GameId);
          }
        }

      }

      DataBind();
    }

    private string GetRandomString(int lenght)
    {
      char[] validChars = "abcdefghijklmnopqrstuvwxyz0123456789".ToCharArray();
      Random random = new Random();
      List<char> result = new List<char>();
      while (result.Count<lenght)
      {
        result.Add(validChars[random.Next(0,validChars.Length)]);
      }
      return string.Join("", result);
    }

    public IEnumerable<Democraticdj.Model.Spotify.Playlist> PlaylistsSource
    {
      get
      {
        IEnumerable<Democraticdj.Model.Spotify.Playlist> results = null;
        using (var currentUser = StateManager.CurrentUser)
        {
          if (currentUser.SpotifyAuthTokens == null)
            return Enumerable.Empty<Democraticdj.Model.Spotify.Playlist>();
          var spotifyUser = SpotifyServices.GetAuthenticatedUser(currentUser.SpotifyAuthTokens);
          results = SpotifyServices.GetPlaylists(currentUser.SpotifyAuthTokens).PlayLists.Where(playlist => playlist.IsPublic && playlist.Owner.Id == spotifyUser.Id).ToArray();
        }
        return results;
      }
    }
  }
}