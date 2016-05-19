<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="Democraticdj._default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
  <title>Democratic DJ</title>
</head>
<body>
  <form id="form1" runat="server">
    <div>
      This might be the initial state, where I am not part of a game yet
    
      prompt me for a game ID to join
       - OR - 
      start a game, authorizing with spotify as needed
       - OR - 
      "See my games", authorizing with spotify as needed
    </div>

    <div>
      <h1>This might be me starting up a game
      </h1>
      <ul>
        <li>being prompted a name for the list
        </li>
        <li>choose an existing list
        </li>
        <li>leaving blank to autogenerate list name
        </li>
      </ul>
    </div>
    <div>
      <h1>this might be a view of the current game in progress
      </h1>

      there should be a link available for sharing
      current standings
      a play button with the list loaded
      a search form thingy
      a search result
    </div>
    <a href="UserManagement.aspx">Go to user management</a>


  </form>
</body>
</html>
