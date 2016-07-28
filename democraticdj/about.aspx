<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="about.aspx.cs" Inherits="Democraticdj.about" %>

<%@ Register Src="~/Controls/HeaderContent.ascx" TagPrefix="uc1" TagName="HeaderContent" %>
<%@ Register Src="~/Controls/PageTop.ascx" TagPrefix="uc1" TagName="PageTop" %>
<!DOCTYPE html>
<html>
<head id="Head1" runat="server">

  <uc1:HeaderContent runat="server" ID="HeaderContent" />
  <script type="text/javascript" src="/scripts/game.js"></script>
</head>
<body>
  <div class="body-sizer">
    <uc1:PageTop runat="server" ID="PageTop" />
    <div class="section">
      <div class="section-header">What is "Democratic DJ"?</div>
      <p>
        Democratic DJ is a bunch of things.<br/>
        It is a game where you compete for the honor of being the best DJ of the crowd.<br/>
        At the same time, Democratic DJ creates a playlist from your game, making it a playlist generator for parties.

      </p>
    </div>
    <div class="section">
      <div class="section-header">What do I need for Democratic DJ to work?</div>
      <p>You need someone who has a Spotify account. Democratic DJ creates a public playlist for the person whom starts a game. For this to work, a Sptify account is needed</p>
      <p>You also need at least 3 participants. Democratic DJ will not determine a winner until 3 votes have been cast</p>
    </div>
    <div class="section">
      <div class="section-header">How do I play this game?</div>
        Once logged in, you create a list of candidates for the playlist. <br />
        Your list is created by searching Spotify, then selecting from the search result. <br />
        Each round, the topmost item on your list will be moved to the candidates list.<br />
        The candidate that receives the most votes wins, and is moved to the playlist.<br />
        The other candidates are moved back to their respective player's lists
    </div>
    <div class="section">
      <div class="section-header">Where do I submit feedback?</div>
        You can tweet <a href="https://twitter.com/NLaust" target="_blank">@NLaust</a>, since he (I) made this. Any feedback appreciated, especially the constructive kind.
    </div>
    <div class="section">
      <div class="section-header"></div>
    </div>
  </div>
</body>
</html>
