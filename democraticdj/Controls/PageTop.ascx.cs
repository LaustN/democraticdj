using System;
using System.Collections.Generic;
using System.Linq;
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

        if (!string.IsNullOrWhiteSpace(currentUser.AvatarUrl))
        {
          UserAvatar.ImageUrl = currentUser.AvatarUrl;
        }
        else
        {
          UserAvatar.ImageUrl = "/graphics/mediaplayer.png";
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
  }
}