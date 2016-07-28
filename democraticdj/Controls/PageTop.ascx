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
      <li class="burger js-burger"><img src="/graphics/burger-menu.svg"/></li>
    </ul>

    <div class="menu js-burger-target hidden">
      <ul class="inner-burger">
        <li class="antiburger js-burger"><img src="/graphics/cross.svg"/></li>
        <li class="menu-line" runat="server" id="CreateGameLink" visible="False">
          <a href="/CreateGame.aspx">Create game</a>
        </li>
        <li class="menu-line">
          <a href="/about.aspx">About Democratic DJ</a>
        </li>
        <asp:PlaceHolder runat="server" ID="LoggedInUser" Visible="False">
          <asp:PlaceHolder runat="server" ID="SpotifyReauthentication" Visible="False">
            <li class="menu-line">
              <a href="<%# RenderSpotifyAuthUrl()%>">Spotify login</a>
            </li>
          </asp:PlaceHolder>
          <li class="menu-line">
            <a href="/UserManagement.aspx">Your details</a>
          </li>
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
