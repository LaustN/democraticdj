﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="authcompleted.aspx.cs" Inherits="Democraticdj.Authcompleted"  ViewStateMode="Disabled"%>

<%@ Import Namespace="System.Web.Http.Controllers" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
  <title></title>
</head>
<body>
  <form id="form1" runat="server">
    <div>
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
  </form>
</body>
</html>
