<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserManagement.aspx.cs" Inherits="Democraticdj.UserManagement" ViewStateMode="Disabled" %>

<%@ Import Namespace="System.Web.Http.Controllers" %>
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
    <div runat="server" class="unknown-user" id="UnknownUser">
      <div class="use-login">
        <h2>Login using... 
        </h2>
        <label>
          <span class="user-label">Email or username</span>
          <input type="text" name="existingUserEmailOrUsername" />
        </label>
        <label>
          <span class="user-label">Password</span>
          <input type="password" name="existingUserPassword" />
        </label>
        <input type="submit" value="OK" />
      </div>

      <div class="use-spotify">
        <h2>Use your Spotify account</h2>
        <a href="<%# Democraticdj.Services.SpotifyServices.GetAuthUrl("/usermanagement.aspx") %>">Click here to sign up with you Spotify account</a>
      </div>

      <div class="create-login">
        <label>
          <span class="user-label">Email</span>
          <input type="text" name="newUserEmail" />
        </label>
        <label>
          <span class="user-label">User name</span>
          <input type="text" name="newUsername" />
        </label>
        <label>
          <span class="user-label">Password</span>
          <input type="password" name="newPassword" />
        </label>
        <input type="submit" value="OK" />
      </div>
    </div>


    <div class="known-user" runat="server" ID="KnownUser">
      <div>
        <label runat="server" id="UserNameAsLabelWrapper">
          <span class="user-label">User name</span>
          <asp:Label runat="server" ID="UserNameAsLabel" />
        </label>
        <label>
          <span class="user-label">Display name</span>
          <input type="text" runat="server" id="NameBox" />
        </label>
        <label>
          <span class="user-label">Password</span>
          <input type="password" runat="server" id="PasswordUpdate" />
        </label>
        <label>
          <span class="user-label">Password</span>
          <input type="password" runat="server" id="PasswordRepeat" />
        </label>
        <input type="submit" value="Save" />
      </div>
      <div>
        <asp:PlaceHolder runat="server" ID="SpotifyAuthLink">
          <a href="<%# Democraticdj.Services.SpotifyServices.GetAuthUrl("/usermanagement.aspx") %>">click here to authenticate</a>
        </asp:PlaceHolder>
        <asp:PlaceHolder runat="server" ID="SpotifyInfo">You are authenticated with spotify 
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
    </div>

  </form>
</body>
</html>
