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
    <form id="MainForm" runat="server" >
      <div>
        <asp:PlaceHolder runat="server" ID="GameNotKnownPlaceholder" Visible="False">
          <h1>Oh no!</h1>
          <div>
            We do not recognize that game. Maybe it has been closed?
          </div>
        </asp:PlaceHolder>
        <asp:Panel runat="server" ID="GameFoundPlaceholder" Visible="False" CssClass="game">
          <div class="ballot section">
            <div class="section-header">
              Like this track?
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
              </div>
            </div>
          </div>
          <asp:PlaceHolder runat="server" ID="AuthenticatedUserControls">
            <div class="search section">
              <div class="section-header">
                Search for tracks, then select from the results to build your list of candidates
              </div>
              <input type="text" class="search-box-js" placeholder="Search Spotify" />
              <div class="search-result-js"></div>
            </div>
            <div class="myplaylists section">
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
              <div class="section-header">Participate</div>
              <p>For the best experience, you should <a href="<%# RenderSpotifyAuthUrl() %>">log in with Spotify by clicking here!</a></p>
              <br/>
              <p>Alternatively, you can just create a new user</p>
              <input name="newplayername" placeholder="Set your player name"/>
              <input type="submit" value="Set my name!"/>
              <p>After you set your player name, you can add candidates to be voted on and gain points!</p>
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
        </asp:Panel>

      </div>
    </form>
  </div>
</body>
</html>
