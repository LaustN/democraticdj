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
    public DateTime? BallotCreationTime { get; set; }
    public DateTime? MinimumVotesCastTime { get; set; }

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

    private List<Vote> _votes;
    public List<Vote> Votes
    {
      get { return _votes ?? (_votes = new List<Vote>()); }
      set { _votes = value; }
    }

    private List<Winner> _previousWinners;
    public List<Winner> PreviousWinners
    {
      get { return _previousWinners ?? (_previousWinners = new List<Winner>()); }
      set { _previousWinners = value; }
    }

    private int _voteClosingDelay;
    public int VoteClosingDelay { get { return _voteClosingDelay > 30 ? _voteClosingDelay : _voteClosingDelay = 30; } set { _voteClosingDelay = value; } }

    private int _minimumVotes;
    public int MinimumVotes
    {
      get { return _minimumVotes > 2 ? _minimumVotes : _minimumVotes = 3; }
      set { _minimumVotes = value; }
    }


  }
}