<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PageTop.ascx.cs" Inherits="Democraticdj.Controls.PageTop" %>
<%@ Import Namespace="System.Web.Http.Controllers" %>
<div class="page-top">
  <div class="top-bar">
    <ul>
      <li class="header-logo">
        <a href="/">
          <img src="/graphics/mediaplayer.png" />
        </a>
      </li>
      <li>
        <a href="/">Democratic DJ
        </a>
      </li>
      <li class="burger js-burger">burger</li>
    </ul>

    <div class="menu js-burger-target hidden">
      <ul class="inner-burger">
        <li class="antiburger js-burger">antiburger</li>
        <li class="menu-line" runat="server" id="CreateGameLink" visible="False">
          <a href="/CreateGame.aspx">Create game</a>
        </li>
        <li class="menu-line">
          <a href="/default.aspx">About Democratic DJ</a>
        </li>
        <asp:PlaceHolder runat="server" ID="LoggedInUser" Visible="False">
          <li class="menu-line">
            <a href="/UserManagement.aspx">Profile</a>
          </li>
          <li class="menu-line">
            <asp:Image runat="server" ID="UserAvatar" CssClass="tiny-avatar" />
          </li>
          <asp:PlaceHolder runat="server" ID="SpotifyReauthentication" Visible="False">
            <li class="menu-line">
              <a href="<%# RenderSpotifyAuthUrl()%>">Spotify login</a>
            </li>
          </asp:PlaceHolder>
        </asp:PlaceHolder>
        <asp:PlaceHolder runat="server" ID="UnknownUser" Visible="False">
          <li class="menu-line">
            <a href="/UserManagement.aspx">Log in</a>
          </li>
          <li class="menu-line">
            <a href="<%# RenderSpotifyAuthUrl()%>">Log in with spotify</a>
          </li>
        </asp:PlaceHolder>
      </ul>

    </div>
  </div>
</div>
