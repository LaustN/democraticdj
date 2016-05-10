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
        var tokens = SpotifyAuthProvider.ProcessAuthCode(authResponseCode, responseState);
        if (tokens != null)
        {
          using (User currentUser = StateManager.CurrentUser)
          {
            currentUser.SpotifyAuthTokens = tokens;
          }
        }
      }

      this.DataBind();

      using (Session session = StateManager.CurrentSession)
      {
        SessionLabel.Text = session.SessionId;
      }
    }
  }
}