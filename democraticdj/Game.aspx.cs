﻿using System;
using System.Linq;
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
        bool userIsAuthenticated = currentUser.IsLoggedIn;
        AuthenticatedUserControls.Visible = userIsAuthenticated;
        UnauthenticatedUser.Visible = !userIsAuthenticated;

        string gameid = Request.QueryString["gameid"];
        CurrentGame = StateManager.Db.GetGame(gameid);

        if (CurrentGame != null)
        {
          GameFoundPlaceholder.Visible = true;
          MainForm.Attributes.Add("data-gameid", gameid);
          GameTitle.Text = CurrentGame.GameName;
          GameID.Text = CurrentGame.GameId;

          if (!CurrentGame.Players.Any(player => player.UserId == currentUser.UserId))
          {
            CurrentGame.Players.Add(new Player{UserId = currentUser.UserId});
          }
        }
        else
        {
          GameNotKnownPlaceholder.Visible = true;
        }
      }


      DataBind();
    }
  }
}