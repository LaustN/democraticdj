using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Remoting.Contexts;
using System.Security.Cryptography;
using System.Threading;
using System.Web;
using System.Web.Http;
using Democraticdj.Logic;
using Democraticdj.Model;
using Democraticdj.Model.Spotify;
using MongoDB.Bson.IO;
using Newtonsoft.Json;

namespace Democraticdj.Services
{

  [RoutePrefix("api")]
  public class RestServicesController : ApiController
  {
    private static bool _flag = false;

    [HttpGet]
    [Route("")]
    public string Get()
    {
      DateTime start = DateTime.Now;
      while (!_flag)
      {
        Thread.Sleep(20);
      }
      _flag = false;
      DateTime stop = DateTime.Now;
      return "gotten " + (stop - start);
    }

    [HttpPost]
    [Route("")]
    //public string Post([FromBody]string value)
    public string Post()
    {
      using (Session session = StateManager.CurrentSession)
      {
        System.Diagnostics.Debug.WriteLine("Session id was " + session.SessionId);
      }

      using (User currentUser = StateManager.CurrentUser)
      {
        System.Diagnostics.Debug.WriteLine("Spotify tokes was " + Newtonsoft.Json.JsonConvert.SerializeObject(currentUser.SpotifyAuthTokens));
      }

      _flag = true;
      return "posted";
    }

    [HttpGet]
    [Route("game/isupdated")]
    public bool CheckGameState(string gameid)
    {
      long initialTick = StateManager.GetGameTick(gameid);
      DateTime initialTime = DateTime.Now;
      while ((DateTime.Now - initialTime).TotalMinutes < 1.0)
      {
        GameLogic.UpdateGameState(gameid);

        long currentTick = StateManager.GetGameTick(gameid);
        if (currentTick != 0 && currentTick != initialTick)
        {
          return true;
        }
        Thread.Sleep(1000);
      }


      return false;
    }

    [HttpGet]
    [Route("game")]
    public GameState GetGameState(string gameid)
    {
      using (User currentUser = StateManager.CurrentUser)
      {
        var game = StateManager.Db.GetGame(gameid);
        GameLogic.UpdateGameState(gameid);

        StateManager.UpdateGameTick(game);

        var player = game.Players.FirstOrDefault(playerScan => playerScan.UserId == currentUser.UserId);
        var gameState = new GameState
        {
          Nominees = game.Nominees.Select(nominee => nominee.TrackId).ToList(),
          PlayerSelectionList = player != null ? player.SelectedTracks.ToList() : null,
          Winners = game.PreviousWinners.Select(winner => winner.TrackId).ToList(),
          PlayersWinners = game.PreviousWinners.Where(previousWinner => previousWinner.SelectingPlayerIds.Contains(currentUser.UserId)).Select(winner => winner.TrackId).ToList(),
          Scores = game.Players
            .OrderBy(aPlayer => -aPlayer.Points)
            .Select(aPlayer=> new PlayerScore
            {
              PlayerId = aPlayer.UserId,Points = aPlayer.Points
            })
            .ToList()
        };
        var playersVote = game.Votes.FirstOrDefault(vote => vote.PlayerId == currentUser.UserId);

        var playerSelectedNominee = game.Nominees.FirstOrDefault(nominee => nominee.NominatingPlayerIds.Contains(currentUser.UserId));

        if (playerSelectedNominee != null)
        {
          gameState.PlayersSelection = playerSelectedNominee.TrackId;
        }

        if (playersVote != null)
        {
          gameState.CurrentVote = playersVote.TrackId;
        }

        gameState.SecondsUntillVoteCloses = game.MinimumVotesCastTime.HasValue
          ? (int)(game.MinimumVotesCastTime.Value.AddSeconds(game.VoteClosingDelay) - DateTime.UtcNow).TotalSeconds
          : -1;
        return gameState;

      }
    }

    [HttpPost]
    [Route("search")]
    public SpotifySearchResponse Search([FromBody]SearchRequest request)
    {
      var game = StateManager.Db.GetGame(request.GameId);
      return SpotifyServices.SearchForTracks(game, request.Query);
    }

    [HttpPost]
    [Route("tracks")]
    public SpotifyGetTracksResponse GetTracks([FromBody]GetTracksRequest request)
    {
      var game = StateManager.Db.GetGame(request.GameId);
      return SpotifyServices.GetTracks(game, request.Tracks);
    }

    [HttpPost]
    [Route("select")]
    public void SelectFromSearchResult([FromBody]SelectRequest request)
    {
      var game = StateManager.Db.GetGame(request.GameId);
      bool tickShouldBeUpdated;
      using (var user = StateManager.CurrentUser)
      {
        tickShouldBeUpdated = GameLogic.SelectTrack(request.GameId, user.UserId, request.TrackId);
      }

      if (tickShouldBeUpdated)
      {
        StateManager.UpdateGameTick(game);
      }
    }

    [HttpPost]
    [Route("vote")]
    public void PlaceVote([FromBody]SelectRequest request)
    {
      var game = StateManager.Db.GetGame(request.GameId);
      bool tickShouldBeUpdated;
      using (var user = StateManager.CurrentUser)
      {
        tickShouldBeUpdated = GameLogic.PlaceVote(request.GameId, user.UserId, request.TrackId);
      }
      StateManager.UpdateGameTick(game);
    }

    [HttpPost]
    [Route("users")]
    public IEnumerable<PublicUser> GetUsers([FromBody]IEnumerable<string> userIds)
    {
      foreach (string userId in userIds)
      {
        var user = StateManager.Db.GetUser(userId);
        var publicUser = new PublicUser
        {
          UserId = user.UserId,
          AvatarUrl = user.AvatarUrl,
          Name = "Guest"
        };
        if (!string.IsNullOrWhiteSpace(user.DisplayName))
        {
          publicUser.Name = user.DisplayName;
        }
        else if (!string.IsNullOrWhiteSpace(user.UserName))
        {
          publicUser.Name = user.UserName;
        }
        else if(user.SpotifyUser != null && !string.IsNullOrWhiteSpace(user.SpotifyUser.DisplayName))
        {
          publicUser.Name = user.SpotifyUser.DisplayName;
        }

        yield return publicUser;
      }
    }

    [HttpPost]
    [Route("verifyemail")]
    public bool VerifyEmail([FromBody]VerifyEmailRequest request)
    {
      using (var user = StateManager.CurrentUser)
      {
        var userEmail =
          user.Emails.FirstOrDefault(
            item => item.Address.Equals(request.Email, StringComparison.InvariantCultureIgnoreCase) && !item.IsVerified);
        if (userEmail != null)
        {
          string newVerificationId = Guid.NewGuid().ToString("N");
          userEmail.PendingVerificationId = newVerificationId;
          

          string handlerUrl =  "http://" + HttpContext.Current.Request.Url.Host + "/usermanagement.aspx?emailverification=" + newVerificationId;

          string mailBodyTemplate = @"
<html>
<body>
Hi {0},<br/>
<a href='{1}'>click here to verify your email: {2}</a>
</body>
</html>
";
          var mailBody = string.Format(mailBodyTemplate, user.DisplayName, handlerUrl, request.Email);
          MailSender.Send("Verify your Democratic DJ email address", mailBody, request.Email, true);
          return true;
        }

      }

      return false;
    }
  }

  public class VerifyEmailRequest
  {
    [JsonProperty("email")]
    public string Email { get; set; }
  }


  public class SelectRequest
  {
    [JsonProperty("gameid")]
    public string GameId { get; set; }

    [JsonProperty("trackid")]
    public string TrackId { get; set; }
  }

  public class SearchRequest
  {
    [JsonProperty("gameid")]
    public string GameId { get; set; }

    [JsonProperty("query")]
    public string Query { get; set; }
  }

  public class GetTracksRequest
  {
    [JsonProperty("gameid")]
    public string GameId { get; set; }

    [JsonProperty("tracks")]
    public string[] Tracks { get; set; }
  }

}
