using System.Collections.Generic;

namespace Democraticdj.Model
{
  public class GameState
  {
    private List<string> _nominees;
    private List<string> _playerSelectionList;
    private List<PlayerScore> _scores;
    public string CurrentVote { get; set; }
    public string PlayersSelection { get; set; }

    public int VotesCastCount;

    private List<string> _playersWinners;
    public List<string> PlayersWinners
    {
      get { return _playersWinners ?? (_playersWinners = new List<string>()); }
      set { _playersWinners = value; }
    }

    private List<string> _winners;
    public List<string> Winners
    {
      get { return _winners ?? (_winners = new List<string>()); }
      set { _winners = value; }
    }

    public List<string> Nominees
    {
      get { return _nominees ?? (_nominees = new List<string>()); }
      set { _nominees = value; }
    }

    public List<string> PlayerSelectionList
    {
      get { return _playerSelectionList ?? (_playerSelectionList = new List<string>()); }
      set { _playerSelectionList = value; }
    }

    public int SecondsUntillVoteCloses;

    public List<PlayerScore> Scores
    {
      get { return _scores??(_scores = new List<PlayerScore>()); }
      set { _scores = value; }
    }
  }

  public class PlayerScore
  {
    public string PlayerId { get; set; }
    public int Points { get; set; }
  }
}