<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Game.aspx.cs" Inherits="Democraticdj.Game" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
  <title>Democratic DJ</title>
  <script type="text/javascript" src="http://code.jquery.com/jquery-1.10.1.min.js"></script>
  <script type="text/javascript" src="/scripts/game.js"></script>
  <link rel="stylesheet" href="styles/democraticdj.css">
</head>
<body>
  <form id="MainForm" runat="server">
    <div>
      <asp:PlaceHolder runat="server" ID="GameNotKnownPlaceholder" Visible="False">
        <h1>Oh no!</h1>
        <div>
          We do not recognize that game. Maybe it has been closed?
        </div>
      </asp:PlaceHolder>
      <asp:Panel runat="server" ID="GameFoundPlaceholder" Visible="False" CssClass="game">
        <div class="ballot">
          <div class="section-header">
            here the current ballot will render
          </div>
          <div class="nominees-list-js"></div>
        </div>
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
