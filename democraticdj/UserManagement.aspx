<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserManagement.aspx.cs" Inherits="Democraticdj.UserManagement"  ViewStateMode="Disabled"%>

<%@ Import Namespace="System.Web.Http.Controllers" %>
<%@ Register Src="~/Controls/HeaderContent.ascx" TagPrefix="uc1" TagName="HeaderContent" %>
<%@ Register Src="~/Controls/PageTop.ascx" TagPrefix="uc1" TagName="PageTop" %>



<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
  <uc1:HeaderContent runat="server" ID="HeaderContent" />
</head>
<body>
  <uc1:PageTop runat="server" id="PageTop" />
  <form id="form1" runat="server">
    <div class="user-management">
      <label>
        <span class="user-label">Shown name</span>
        <input type="text" runat="server" id="NameBox" />
      </label>
      <label>
        <span class="user-label">Email</span>
        <input type="email" runat="server" id="EmailBox" />
      </label>
      <label>
        <span class="user-label">Password</span>
        <input type="text" runat="server" id="PasswordBox" />
      </label>
      <input type="submit" value="Save" />
      <asp:PlaceHolder runat="server" ID="SpotifyAuthLink">
        <a href="<%# Democraticdj.Services.SpotifyServices.GetAuthUrl("anything") %>">click here to authenticate</a>
      </asp:PlaceHolder>
      <asp:PlaceHolder runat="server" ID="SpotifyInfo">
        You are authenticated with spotify 
      </asp:PlaceHolder>
    </div>
    <div>
      <a href="/CreateGame.aspx">Create new game</a>
    </div>
    <div class="existing-games">
      <asp:Repeater runat="server" ID="ExistingGamesRepeater" ItemType="Democraticdj.Model.Game" DataSource="<%# ExistingGames %>">
        <ItemTemplate>
          <div>
            <a href="Game.aspx?gameid=<%# Item.GameId %>">
              <%# Item.GameName %>
            </a>
          </div>
        </ItemTemplate>
      </asp:Repeater>
    </div>

  </form>
</body>
</html>
