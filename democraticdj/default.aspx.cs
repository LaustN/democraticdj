using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Democraticdj.Services;

namespace Democraticdj
{
  public partial class _default : System.Web.UI.Page
  {
    protected string CurrentUserId;

    
    protected void Page_Load(object sender, EventArgs e)
    {
      if (IsPostBack)
      {
        string joinGameId = Request.Form["joinGameId"];
        if (!string.IsNullOrWhiteSpace(joinGameId))
        {
          joinGameId = joinGameId.ToLower().Trim();
          var game = StateManager.Db.GetGame(joinGameId);
          if (game!=null)
          {
            Response.Redirect("/Game.aspx?gameid=" + joinGameId);
          }
          else
          {
            NoSuchGame.Visible = true;
          }
        }
      }

      using(var currentUser =  StateManager.CurrentUser)
      {
        CurrentUserId = currentUser.UserId;
        if (currentUser.IsLoggedIn)
        {
          UnauthenticatedUser.Visible = false;
        }
        else
        {
          IsAuthenticatedUser.Visible = false;
        }
      }
      DataBind();
    }

    protected string RenderSpotifyAuthUrl()
    {
      Uri originalUri = new Uri(Request.Url.AbsoluteUri);

      string modifiedUrl = originalUri.GetLeftPart(UriPartial.Authority) + originalUri.LocalPath +
                           originalUri.Query;

      return Democraticdj.Services.SpotifyServices.GetAuthUrl(modifiedUrl);
    }

    public IEnumerable<Model.Game> ExistingGames
    {
      get
      {
          return StateManager.Db.FindGamesByParticipatingUser(CurrentUserId);
      }
    }


  }
}