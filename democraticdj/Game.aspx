﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Game.aspx.cs" Inherits="Democraticdj.Game" %>

<%@ Register Src="~/Controls/HeaderContent.ascx" TagPrefix="uc1" TagName="HeaderContent" %>
<%@ Register Src="~/Controls/PageTop.ascx" TagPrefix="uc1" TagName="PageTop" %>
<!DOCTYPE html>
<html>
<head runat="server">

  <uc1:HeaderContent runat="server" ID="HeaderContent" />
  <script type="text/javascript" src="/scripts/game.js"></script>
</head>
<body>

  <div class="body-sizer">
    <uc1:PageTop runat="server" ID="PageTop" />
    <form id="MainForm" runat="server">
      <div>
        <asp:PlaceHolder runat="server" ID="GameNotKnownPlaceholder" Visible="False">
          <h1>Oh no!</h1>
          <div>
            We do not recognize that game. Maybe it has been closed?
          </div>
        </asp:PlaceHolder>
        <asp:Panel runat="server" ID="GameFoundPlaceholder" Visible="False" CssClass="game">
          <h3>Game name:</h3>
          <h1>
            <asp:Label runat="server" ID="GameTitle" /></h1>
          <br />
          <h3>Game ID:</h3>
          <h2>
            <asp:Label runat="server" ID="GameID" /></h2>
          <br />
          <div class="leaderboard">
            <div class="section-header">
              Leaderboard
            </div>
            <div class="player-scores-js" />
          </div>
          <div class="ballot">
            <div class="section-header">
              Voting ballot
            </div>
            <div class="voting-countdown-js"></div>
            <div class="nominees-list-js"></div>
          </div>
          <asp:PlaceHolder runat="server" ID="AuthenticatedUserControls">
            <div class="mylist">
              <div class="section-header">
                My candidates
              </div>
              <div class="player-list-js"></div>
            </div>
            <div class="search">
              <div class="section-header">
                Search for tracks
              </div>
              <input type="text" class="search-box-js" placeholder="Search Spotify for great tracks" />
              <div class="search-result-js"></div>
            </div>

          </asp:PlaceHolder>
          <asp:PlaceHolder runat="server" ID="UnauthenticatedUser">
            <div>
              If you log in, you can add songs to the voting ballot
            </div>
          </asp:PlaceHolder>
          <div>
            <div class="section-header">
              <a href="<%# CurrentGame.SpotifyPlaylistUri %>">Click here to launch the playlist in Spotify</a>
            </div>
          </div>
          <div class="winners">
            <div class="section-header">
              Winning tracks of this game <span class="players-winner">(Your winning tracks)</span>
            </div>
            <div class="winners-list-js">None yet</div>
          </div>
        </asp:Panel>

      </div>
    </form>
  </div>
</body>
</html>
