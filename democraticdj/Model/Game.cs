using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace Democraticdj.Model
{
  public class Game
  {
    public MongoDB.Bson.ObjectId Id { get; set; }
    public string UserId;
    public string SpotifyPlaylistId;
    public string SpotifyPlaylistUri;
    public string GameId;
    public string GameName;
    public DateTime? GameStateUpdateTime { get; set; }

    private List<Nominee> _nominees;
    public List<Nominee> Nominees
    {
      get { return _nominees ?? (_nominees = new List<Nominee>()); }
      set { _nominees = value; }
    }

    private List<Player> _players;
    public List<Player> Players
    {
      get { return _players ?? (_players = new List<Player>()); }
      set { _players = value; }
    }
  }
}