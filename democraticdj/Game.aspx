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
          <div class="section">
            <h3>Game name:</h3>
            <h1>
              <asp:Label runat="server" ID="GameTitle" /></h1>
            <br />
            <h3>Game ID:</h3>
            <h2>
              <asp:Label runat="server" ID="GameID" /></h2>
            <br />
            <div class="section-header">
              <a href="<%# CurrentGame!=null ?  CurrentGame.SpotifyPlaylistUri : String.Empty %>">Click here to launch the playlist in Spotify</a>
            </div>
          </div>
          <div class="ballot section">
            <div class="section-header">
              Current playlist candidates (one candidate from each contributing player)
              <div class=" section-header legend">
                <span class="current-vote">Your current vote (click a track to vote)</span>
                <span class="players-selection">Your candidate (vote on some other track)</span>
              </div>
            </div>
            <div class="votes-cast votes-cast-js"></div>
            <div class="voting-countdown voting-countdown-js"></div>
            <div class="nominees-list-js"></div>
          </div>
          <asp:PlaceHolder runat="server" ID="AuthenticatedUserControls">
            <div class="mylist section">
              <div class="section-header">
                Your candidates (click a track to move it up or remove it)
              </div>
              <div class="player-list-js"></div>
            </div>
            <div class="search section">
              <div class="section-header">
                Search for tracks, then select from the results to build your list of candidates
              </div>
              <input type="text" class="search-box-js" placeholder="Search Spotify for great tracks" />
              <div class="search-result-js"></div>
            </div>

          </asp:PlaceHolder>
          <asp:PlaceHolder runat="server" ID="UnauthenticatedUser">
            <div>
              <div class="section-header">Log in to participate</div>
              <p>If you log in, you can build a list of candidates for the playlist. </p>
              <p>Each round, the topmost item on your list will be moved to the candidates list.</p>
              <p>The candidate that receives the most votes wins, and is moved to the playlist.</p>
              <p>The other candidates are moved back to their respective player's lists</p>
            </div>
          </asp:PlaceHolder>
          <div class="leaderboard section">
            <div class="section-header">
              Leaderboard
            </div>
            <div class="player-scores-js"></div>
          </div>
          <div class="winners section">
            <div class="section-header">
              Winning tracks of this game 
              <div class=" section-header legend">
                <span class="players-winner">Your winning tracks</span>
              </div>
            </div>
            <div class="winners-list-js">None yet</div>
          </div>
        </asp:Panel>

      </div>
    </form>
  </div>
</body>
</html>
