using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Democraticdj.Services;

namespace Democraticdj
{
  public partial class Game : System.Web.UI.Page
  {
    protected Model.Game CurrentGame;
    protected void Page_Load(object sender, EventArgs e)
    {
      string gameid = Request.QueryString["gameid"];
      CurrentGame = StateManager.Db.GetGame(gameid);

      if (CurrentGame != null)
      {
        this.GameFoundPlaceholder.Visible = true;
      }
      else
      {
        this.GameNotKnownPlaceholder.Visible = true;
      }

      DataBind();
    }
  }
}