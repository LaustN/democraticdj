<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="Democraticdj._default" %>

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
    <div runat="server" id="UnauthenticatedUser">
      Welcome to the democratic DJ!
      <br />
      The goal of this game is to become the best DJ of te party, by selecting the spotify tracks that the most people want to hear.<br />
      In order to participate, you need to log in.<br />
      You can create a profile using <a href="/UserManagement.aspx">username, e-mail and password</a>
      , or you can simplify everything by <a href="<%# RenderSpotifyAuthUrl() %>">logging in with spotify</a> right away. 
    </div>
    <div runat="server" id="AuthenticatedUser">
      <h2>Join a game!</h2>
      <input type="text" name="joinGameId" placeholder="Type the ID of the game you wish to join"/>
      <input type="submit" value="join"/>
      <asp:Label runat="server" ID="NoSuchGame" Visible="False">I'm sorry, I could not find that game</asp:Label>
      
      <h2>Your current games</h2>
      <div class="existing-games">
        <asp:Repeater runat="server" ID="ExistingGamesRepeater" ItemType="Democraticdj.Model.Game" DataSource="<%# ExistingGames %>">
          <ItemTemplate>
            <div>
              <a href="Game.aspx?gameid=<%# Item.GameId %>">
                <%# Item.GameName %> - Game ID: <%# Item.GameId %>
              </a>
            </div>
          </ItemTemplate>
        </asp:Repeater>
      </div>
    </div>
    <a href="UserManagement.aspx">Go to user management</a>

  </form>
</body>
</html>
