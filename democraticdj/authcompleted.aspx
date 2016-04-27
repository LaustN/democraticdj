<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="authcompleted.aspx.cs" Inherits="Democraticdj.Authcompleted" %>
<%@ Import Namespace="System.Web.Http.Controllers" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <a href="<%# Democraticdj.Services.SpotifyAuthProvider.GetAuthUrl("anything") %>">click here to authenticate</a>
    </div>
    </form>
</body>
</html>
