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
        <h1><asp:Label runat="server" ID="GameTitle" /></h1>
        <h2>Game ID: <asp:Label runat="server" ID="GameID" /></h2>
        <div class="ballot">
          <div class="section-header">
            here the current ballot will render
          </div>
          <div class="nominees-list-js"></div>
        </div>
        <asp:PlaceHolder runat="server" ID="AuthenticatedUserControls">
          <div class="search">
            <input type="text" class="search-box-js" />
            <div class="search-result-js"></div>
          </div>
          <div class="mylist">
            <div class="section-header">
              here the list of "my songs" will render
            </div>
            <div class="player-list-js"></div>
          </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder runat="server" ID="UnauthenticatedUser">
          <div>
            If you log in, you can add songs to the voting ballot
          </div>
        </asp:PlaceHolder>
        <div>
          <div class="section-header">
            play button goes here
          </div>
          <iframe src="https://embed.spotify.com/?uri=<%# CurrentGame != null ? CurrentGame.SpotifyPlaylistUri : string.Empty %>" width="300" height="80" frameborder="0" allowtransparency="true"></iframe>
        </div>
      </asp:Panel>

    </div>
  </form>
</body>
</html>
