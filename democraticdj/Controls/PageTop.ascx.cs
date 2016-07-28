using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Democraticdj.Services;

namespace Democraticdj.Controls
{
  public partial class PageTop : System.Web.UI.UserControl
  {
    protected void Page_Load(object sender, EventArgs e)
    {
      using (var currentUser = StateManager.CurrentUser)
      {

        if (currentUser.IsLoggedIn)
        {
          LoggedInUser.Visible = true;
          if (currentUser.SpotifyAuthTokens == null)
          {
            SpotifyReauthentication.Visible = true;
          }
          else
          {
            CreateGameLink.Visible = true;
          }
        }
        else
        {
          UnknownUser.Visible = true;
        }
      }
      DataBind();
    }

    protected string RenderSpotifyAuthUrl()
    {
      Regex portFinder=  new Regex(":\\d+");

      string modifiedUrl = portFinder.Replace(Request.Url.AbsoluteUri, "");

      return SpotifyServices.GetAuthUrl(modifiedUrl);
    }
  }
}