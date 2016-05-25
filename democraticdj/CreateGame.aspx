<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CreateGame.aspx.cs" Inherits="Democraticdj.CreateGame" ResponseEncoding="UTF-8" %>

<%@ Register Src="~/Controls/HeaderContent.ascx" TagPrefix="uc1" TagName="HeaderContent" %>
<%@ Register Src="~/Controls/PageTop.ascx" TagPrefix="uc1" TagName="PageTop" %>



<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
  <uc1:HeaderContent runat="server" ID="HeaderContent" />
</head>
<body>
  <uc1:PageTop runat="server" ID="PageTop" />
  <form id="form1" runat="server">
    <div>
      <label>
        <span>List Name</span>
        <input type="text" runat="server" id="ListName" />
      </label>
    </div>
    <input type="submit" value="Create game" />
    <asp:Label runat="server" ID="Result"></asp:Label>
  </form>
  <asp:Repeater runat="server" ID="Playlists" DataSource="<%# PlaylistsSource %>" ItemType="Democraticdj.Model.Spotify.Playlist">
    <ItemTemplate>
      <div class="create-game">
        <div>
          <a href="?createfromlistid=<%# Item.Id %>">
            Create game from playlist 
            <%# Item.Name %>
          </a>
          <iframe src="https://embed.spotify.com/?uri=<%# Item.Uri %>" width="300" height="80" frameborder="0" allowtransparency="true"></iframe>
        </div>
      </div>
    </ItemTemplate>
  </asp:Repeater>
</body>
</html>
