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

    [HttpPost]
    [Route("search")]
    public SpotifySearchResponse Search([FromBody]SearchRequest request)
    {
      var game = StateManager.Db.GetGame(request.GameId);
      return SpotifyServices.SearchForTracks(game, request.Query);
    }

    [HttpPost]
    [Route("select")]
    public void SelectFromSearchResult([FromBody]SelectRequest request)
    {
      var game = StateManager.Db.GetGame(request.GameId);
      using (var user = StateManager.CurrentUser)
      {
        GameLogic.SelectTrack(game,user.UserId,request.TrackId);
      }

      StateManager.Db.SaveGame(game);
    }

    [HttpPost]
    [Route("vote")]
    public void PlaceVote([FromBody]SelectRequest request)
    {
      var game = StateManager.Db.GetGame(request.GameId);
      using (var user = StateManager.CurrentUser)
      {
        GameLogic.SelectTrack(game,user.UserId,request.TrackId);
      }

      StateManager.Db.SaveGame(game);
    }


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

}