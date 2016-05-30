using System.Collections.Generic;

namespace Democraticdj.Model
{
  public class Winner
  {
    public string TrackId;
    private List<string> _selectingPlayerIds;
    public List<string> SelectingPlayerIds { get
    {
      return _selectingPlayerIds ?? (_selectingPlayerIds = new List<string>());
    } set { _selectingPlayerIds = value; } } 
  }
}