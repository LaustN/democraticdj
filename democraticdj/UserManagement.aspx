<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserManagement.aspx.cs" Inherits="Democraticdj.UserManagement" ViewStateMode="Disabled" %>

<%@ Import Namespace="System.Web.Http.Controllers" %>
<%@ Register Src="~/Controls/HeaderContent.ascx" TagPrefix="uc1" TagName="HeaderContent" %>
<%@ Register Src="~/Controls/PageTop.ascx" TagPrefix="uc1" TagName="PageTop" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
  <uc1:HeaderContent runat="server" ID="HeaderContent" />
  <script type="text/javascript" src="/scripts/userManagement.js"></script>
</head>
<body>
  <div class="body-sizer">
    <uc1:PageTop runat="server" ID="PageTop" />
    <form id="form1" runat="server">
      <div runat="server" class="unknown-user section" id="UnknownUser">
        <fieldset class="use-login">
          <div class="section-header">
            Log in
          </div>
          <label>
            <span class="user-label">Email or username</span>
            <input type="text" name="existingUserEmailOrUsername" />
          </label>
          <label>
            <span class="user-label">Password</span>
            <input type="password" name="existingUserPassword" />
          </label>
          <input type="submit" value="OK" />
        </fieldset>

        <div class="use-spotify section">
          <div class="section-header">Use your Spotify account</div>
          <a href="<%# Democraticdj.Services.SpotifyServices.GetAuthUrl("/usermanagement.aspx") %>">Click here to sign up with you Spotify account</a>
        </div>

        <div class="create-login section">
          <div class="section-header">Create new login using plain credentials</div>
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


      <div class="known-user section" runat="server" id="KnownUser">
        <div class="section">
          <div class="section-header">Edit your rofile</div>
          <label runat="server" id="UserNameAsLabelWrapper">
            <span class="user-label">User name</span>
            <asp:Label runat="server" ID="UserNameAsLabel" />
          </label>
          <label>
            <span class="user-label">Display name</span>
            <input type="text" runat="server" id="NameBox" />
          </label>
          <label>
            <span class="user-label">Change password</span>
            <input type="password" runat="server" id="PasswordUpdate" />
          </label>
          <label>
            <span class="user-label">Change password, repeat</span>
            <input type="password" runat="server" id="PasswordRepeat" />
          </label>
          <input type="submit" value="Save" />
        </div>
        <div class="emails section">
          <div class="section-header">Your email addresses</div>
          <asp:Repeater runat="server" DataSource="<%# Emails %>" ItemType="Democraticdj.Model.UserEmail">
            <ItemTemplate>
              <div data-address="<%# Item.Address %>" class="email <%# Item.IsVerified ? "verified" : "notverified" %>">
                <%# Item.Address %>
              </div>
            </ItemTemplate>
          </asp:Repeater>
          <asp:PlaceHolder runat="server" Visible="<%# !Emails.Any() %>">you do not have any email adresses registered
          </asp:PlaceHolder>
        </div>
        <asp:PlaceHolder runat="server" Visible="<%# GravatarOptions.Any() %>">

          <asp:Repeater runat="server" DataSource="<%# GravatarOptions %>" ItemType="Democraticdj.IconOption">
            <HeaderTemplate>
              <div class="iconoptions section">
                <div class="section-header">Choose an Avatar <a href="https://en.gravatar.com/">(customize your email based avatar at https://en.gravatar.com/ )</a>)</div>
            </HeaderTemplate>
            <ItemTemplate>
              <a class="icon-option<%# Item.Url == CurrentUserAvatarUrl ? " active": "" %>" href="<%# "?useIcon=" + Item.Index %>">
                <img src="<%# Item.Url %>" />
              </a>
            </ItemTemplate>
            <FooterTemplate>
              </div>
            </FooterTemplate>
          </asp:Repeater>
        </asp:PlaceHolder>
        <div class="section">
          <asp:PlaceHolder runat="server" ID="SpotifyAuthLink">
            <div class="section-header">
              <a class="fake-button" href="<%# Democraticdj.Services.SpotifyServices.GetAuthUrl("/usermanagement.aspx") %>">Authenticate with Spotify</a>
            </div>
            <div>
              You need to be authenticated with Spotify, if you want to start a new game.<br />
              You may join other games without logging in with spotify - just ask another participant (or the game owner) about the game id or a link to their game.
            </div>
          </asp:PlaceHolder>
          <asp:PlaceHolder runat="server" ID="SpotifyInfo">
            <div class="section-header">You are authenticated with spotify </div>
            <div>
              <a class="fake-button" href="/CreateGame.aspx">Create new game</a>
            </div>
          </asp:PlaceHolder>
        </div>
        <div class="existing-games section">
          <div class="section-header">Your games</div>
          <asp:Repeater runat="server" ID="ExistingGamesRepeater" ItemType="Democraticdj.Model.Game" DataSource="<%# ExistingGames %>">
            <ItemTemplate>
              <div>
                <a class="existing-game" href="Game.aspx?gameid=<%# Item.GameId %>">
                  <%# Item.GameName %> - Game ID: <%# Item.GameId %>
                </a>
              </div>
            </ItemTemplate>
          </asp:Repeater>
          <div runat="server" visible="<%# !ExistingGames.Any() %>">You are not currently part of any games.</div>
        </div>
      </div>

    </form>
  </div>
</body>
</html>
