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
  public partial class Authcompleted : System.Web.UI.Page
  {
    protected void Page_Load(object sender, EventArgs e)
    {
      var authResponseCode = Request.QueryString["code"];
      var responseState = Request.QueryString["state"];
      if (!string.IsNullOrEmpty(authResponseCode))
      {
        var tokens = SpotifyServices.ProcessAuthCode(authResponseCode, responseState);
        if (tokens != null)
        {
          tokens.ReceivedTime = DateTime.UtcNow;
          using (User currentUser = StateManager.CurrentUser)
          {
            currentUser.SpotifyAuthTokens = tokens;
          }
        }
      }
      using (User user = StateManager.CurrentUser)
      {
        if (IsPostBack)
        {
          if (!string.IsNullOrWhiteSpace(NameBox.Value) && NameBox.Value != user.DisplayName)
          {
            user.DisplayName = NameBox.Value;
          }

          if (!string.IsNullOrWhiteSpace(EmailBox.Value) && EmailBox.Value != user.Email)
          {
            user.Email = EmailBox.Value;
          }

          if (!string.IsNullOrWhiteSpace(PasswordBox.Value) && PasswordBox.Value != user.Password)
          {
            user.Password = PasswordBox.Value;
          }
        }
        else
        {
          NameBox.Value = user.DisplayName;
          EmailBox.Value = user.Email;
          PasswordBox.Value = user.Password;
          PasswordBox.Attributes["type"] = "password";

        }

        if (user.SpotifyAuthTokens != null && !string.IsNullOrWhiteSpace(user.SpotifyAuthTokens.AccessToken))
        {
          SpotifyAuthLink.Visible = false;
          SpotifyInfo.Visible = true;
        }
        else
        {
          SpotifyAuthLink.Visible = true;
          SpotifyInfo.Visible = false;
        }
      }
      DataBind();
    }
  }
}