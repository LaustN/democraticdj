using System.Collections.Generic;

namespace Democraticdj.Model
{
  public class GameState
  {
    private List<string> _nominees;
    private List<string> _playerSelectionList;
    private List<string> _winners;
    public string CurrentVote { get; set; }

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
  }
}