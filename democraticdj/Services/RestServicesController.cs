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

    [HttpGet]
    [Route("game/isupdated")]
    public bool CheckGameState(string gameid)
    {
      long initialTick = StateManager.GetGameTick(gameid);
      DateTime initialTime = DateTime.Now;
      while ((DateTime.Now - initialTime).TotalMinutes < 1.0)
      {
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
        StateManager.UpdateGameTick(game);

        var player = game.Players.FirstOrDefault(playerScan => playerScan.UserId == currentUser.UserId);

        var winningTracks = game.Nominees
          .Where(nominee => (nominee.UpVotes.Count + nominee.NominatingPlayerIds.Count) > nominee.DownVotes.Count).ToArray();

        var scoresDictionary = new Dictionary<string, PlayerScore>();
        foreach (Nominee winningTrack in winningTracks)
        {
          foreach (var nominatingPlayerId in winningTrack.NominatingPlayerIds)
          {
            if (scoresDictionary.ContainsKey(nominatingPlayerId))
            {
              scoresDictionary[nominatingPlayerId].Points++;
            }
            else
            {
              scoresDictionary.Add(nominatingPlayerId, new PlayerScore { PlayerId = nominatingPlayerId, Points = 1 });
            }
          }
        }

        var otherPlayersNominees = game.Nominees.Where(nominee => !nominee.UpVotes.Contains(player.UserId) && !nominee.DownVotes.Contains(player.UserId) && !nominee.NominatingPlayerIds.Contains(player.UserId)).ToList();
        var thisPlayersNominees = game.Nominees.Where(nominee => nominee.NominatingPlayerIds.Contains(player.UserId) ).ToList();

        var gameState = new GameState
        {
          PlayerSelectionList = player != null ? thisPlayersNominees.Select(nominee=>nominee.TrackId).ToList() : null,
          Winners = winningTracks.Select(winner => winner.TrackId).ToList(),
          PlayersWinners = winningTracks.Where(nomineee => nomineee.NominatingPlayerIds.Contains(player.UserId)).Select(winner => winner.TrackId).ToList(),
          Scores = scoresDictionary.Values.OrderByDescending(score=>score.Points).ToList(),
        };

        Random random = new Random();
        while (otherPlayersNominees.Count>0)
        {
          int selectedIndex = random.Next(0, otherPlayersNominees.Count);
          gameState.Nominees.Add(otherPlayersNominees[selectedIndex].TrackId);
          otherPlayersNominees.RemoveAt(selectedIndex);
        }
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
    public void SelectTrack([FromBody]SelectRequest request)
    {
      var game = StateManager.Db.GetGame(request.GameId);
      using (var user = StateManager.CurrentUser)
      {
        GameLogic.SelectTrack(request.GameId, user.UserId, request.TrackId);
      }
      StateManager.UpdateGameTick(game);
    }

    [HttpPost]
    [Route("unselect")]
    public void UnselectTrack([FromBody]SelectRequest request)
    {
      var game = StateManager.Db.GetGame(request.GameId);
      using (var user = StateManager.CurrentUser)
      {
        GameLogic.SelectTrack(request.GameId, user.UserId, request.TrackId, true);
      }

      StateManager.UpdateGameTick(game);
    }

    [HttpPost]
    [Route("vote")]
    public void PlaceVote([FromBody]VoteRequest request)
    {
      var game = StateManager.Db.GetGame(request.GameId);
      using (var user = StateManager.CurrentUser)
      {
        GameLogic.PlaceVote(request.GameId, user.UserId, request.TrackId, request.IsUpVote);
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
        else if (user.SpotifyUser != null && !string.IsNullOrWhiteSpace(user.SpotifyUser.DisplayName))
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


          string handlerUrl = "http://" + HttpContext.Current.Request.Url.Host + "/usermanagement.aspx?emailverification=" + newVerificationId;

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

  public class VoteRequest
  {
    [JsonProperty("gameid")]
    public string GameId { get; set; }

    [JsonProperty("trackid")]
    public string TrackId { get; set; }

    [JsonProperty("isupvote")]
    public bool IsUpVote { get; set; }
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
