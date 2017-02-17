using System;
using System.Linq;
using System.Text.RegularExpressions;
using Democraticdj.Model;
using Democraticdj.Services;

namespace Democraticdj
{
  public partial class Game : System.Web.UI.Page
  {
    protected Model.Game CurrentGame;
    protected void Page_Load(object sender, EventArgs e)
    {
      using (var currentUser = StateManager.CurrentUser)
      {
        if (IsPostBack)
        {
          string newPlayerName = Request.Form["newplayername"];
          if (!string.IsNullOrWhiteSpace(newPlayerName))
          {
            currentUser.UserName = newPlayerName;
          }
        }

        bool userIsAuthenticated = currentUser.IsLoggedIn;
        AuthenticatedUserControls.Visible = userIsAuthenticated;
        UnauthenticatedUser.Visible = !userIsAuthenticated;

        string gameid = Request.QueryString["gameid"];
        CurrentGame = StateManager.Db.GetGame(gameid);

        if (CurrentGame != null)
        {
          GameFoundPlaceholder.Visible = true;
          MainForm.Attributes.Add("data-gameid", gameid);
          MainForm.Action = "/Game.aspx?gameid=" + gameid;
          GameTitle.Text = CurrentGame.GameName;
          GameID.Text = CurrentGame.GameId;


          if (!CurrentGame.Players.Any(player => player.UserId == currentUser.UserId))
          {
            CurrentGame.Players.Add(new Player { UserId = currentUser.UserId });
          }
        }
        else
        {
          GameNotKnownPlaceholder.Visible = true;
        }
      }


      DataBind();
    }

    protected string RenderSpotifyAuthUrl()
    {
      Regex portFinder = new Regex(":\\d+");
      string absoluteUrl = Request.Url.AbsoluteUri;
      string modifiedUrl = portFinder.Replace(absoluteUrl, "");

      return SpotifyServices.GetAuthUrl(modifiedUrl);
    }

  }
}