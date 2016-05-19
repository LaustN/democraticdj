<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Game.aspx.cs" Inherits="Democraticdj.Game" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
  <title>Democratic DJ</title>
</head>
<body>
  <form id="form1" runat="server">
    <div>
      <asp:PlaceHolder runat="server" ID="GameNotKnownPlaceholder" Visible="False">
        <h1>Oh no!</h1>
        <div>
         We do not recognize that game. Maybe it has been closed?
        </div>
      </asp:PlaceHolder>
      <asp:Panel runat="server" ID="GameFoundPlaceholder" Visible="False" CssClass="game">
        <h1>RIGHT ON!</h1>
        <div>
          some header stuff
        </div>
        <div class="ballot">
          here the current ballot will render
        </div>
        <div class="search">
          here I'll implement search with clickable songs
        </div>
        <div class="mylist">
          here the list of "my songs" will render
        </div>
        <div>
          play button goes here<br/>
          <iframe src="https://embed.spotify.com/?uri=<%# CurrentGame != null ? CurrentGame.SpotifyPlaylistUri : string.Empty %>" width="300" height="80" frameborder="0" allowtransparency="true"></iframe>
        </div>
      </asp:Panel>

    </div>
  </form>
</body>
</html>
