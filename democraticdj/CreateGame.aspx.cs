using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Democraticdj.Services;

namespace Democraticdj
{
  public partial class CreateGame : System.Web.UI.Page
  {
    protected void Page_Load(object sender, EventArgs e)
    {
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