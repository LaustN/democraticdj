<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Game.aspx.cs" Inherits="Democraticdj.Game" %>

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
              <a href="<%# CurrentGame!=null ?  CurrentGame.SpotifyPlaylistUri : String.Empty %>">
                <span class="small-image">
                  <img src="/graphics/Spotify.svg" /></span>Press play!<span class="small-image"><img src="/graphics/play.svg" /></span>
              </a>
            </div>
          </div>
          <div class="ballot section">
            <div class="section-header">
              Like this track? - click or push image to preview
            </div>
            <div class="nominees-list-js"></div>
            <div class="sharethisgame">
              <div class="section-header">
                There is nothing for you to vote on right now. Share this game!
              </div>
              <div class="shareoption">
                <div class="shareaction mail">
                  <a href="mailto:?body=body+of+the+mail&subject=subject"><img src="/graphics/envelope.svg"/></a>
                </div>
                <div class="shareaction sms">
                  <a href="sms:&body=body+of+the+mail"><img src="/graphics/message.svg"/></a>
                </div>
                <div class="shareinput copy">
                  <input type="text" disabled="disabled" value="disabled" />
                </div>
              </div>
            </div>
          </div>
          <asp:PlaceHolder runat="server" ID="AuthenticatedUserControls">
            <div class="search section">
              <div class="section-header">
                Search for tracks, then select from the results to build your list of candidates
              </div>
              <input type="text" class="search-box-js" placeholder="Search Spotify for great tracks" />
              <div class="search-result-js"></div>
            </div>
            <div class="mylist section">
              <div class="section-header">
                Your candidates 
              </div>
              <div class="player-list-js"></div>
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
