<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CreateGame.aspx.cs" Inherits="Democraticdj.CreateGame" ResponseEncoding="UTF-8" %>

<%@ Register Src="~/Controls/HeaderContent.ascx" TagPrefix="uc1" TagName="HeaderContent" %>
<%@ Register Src="~/Controls/PageTop.ascx" TagPrefix="uc1" TagName="PageTop" %>



<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
  <uc1:HeaderContent runat="server" ID="HeaderContent" />
</head>
<body>
  <div class="body-sizer">
    <uc1:PageTop runat="server" ID="PageTop" />
    <form id="form1" runat="server">
      <div>
        <label>
          <span>You may create a game by entering a game name in this box. 
          <br />
            A public playlist with the entered name will be created, and a game session using the newly created playlist will be created.
          </span>
          <input type="text" runat="server" id="ListName" />
        </label>
      </div>
      <input type="submit" value="Create game" />
      <asp:Label runat="server" ID="Result"></asp:Label>
    </form>

    <h2>Click one of your playlists below, if you want to start a game session using that list. 
    </h2>
    <div class="create-game-from-existing-list">
      <asp:Repeater runat="server" ID="Playlists" DataSource="<%# PlaylistsSource %>" ItemType="Democraticdj.Model.Spotify.Playlist">
        <ItemTemplate>
          <div class="existing-list">
            <a href="?createfromlistid=<%# Item.Id %>">
              <div class="existing-list-name">
                <div class="existing-list-name-verticalizer">
                  <%# Item.Name %>
                </div>
              </div>
              <div class="existing-list-image">
                <img src="<%# Item.Images.Any() ? Item.Images.First().Url : string.Empty %>" />
              </div>
            </a>
          </div>
        </ItemTemplate>
      </asp:Repeater>
    </div>
  </div>
</body>
</html>
