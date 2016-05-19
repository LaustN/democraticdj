using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Democraticdj.Model;
using Democraticdj.Services;

namespace Democraticdj
{
  public partial class CreateGame : System.Web.UI.Page
  {
    protected void Page_Load(object sender, EventArgs e)
    {
      var createFromListId = Request.QueryString["createfromlistid"];
      if (!string.IsNullOrWhiteSpace(createFromListId))
      {

        using (var user = StateManager.CurrentUser)
        {
          var playlist =
            SpotifyServices.GetPlaylists(user.SpotifyAuthTokens)
              .PlayLists.FirstOrDefault(playList => playList.Id == createFromListId);
          if (playlist != null)
          {
            var game = new Model.Game
            {
              GameId = Guid.NewGuid().ToString("N"),
              SpotifyPlaylistId = createFromListId,
              SpotifyPlaylistUri = playlist.Uri,
              UserId = user.UserId,
              GameName = playlist.Name + " - " + (user.DisplayName ?? user.SpotifyUser.DisplayName)
            };
            StateManager.Db.SaveGame(game);
            Response.Redirect("/Game.aspx?gameid=" + game.GameId);
          }
        }

      }
      DataBind();
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