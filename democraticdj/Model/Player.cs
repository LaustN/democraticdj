using System.Collections.Generic;

namespace Democraticdj.Model
{
  public class Player
  {
    public string UserId;

    private List<string> _selectedTracks; //just a set of spotify track IDs
    public List<string> SelectedTracks
    {
      get { return _selectedTracks ??(_selectedTracks = new List<string>()); }
      set { _selectedTracks = value; }
    }

    public int Points;
  }
}