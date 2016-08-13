using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Democraticdj.Model
{
  public class Nominee
  {
    public string TrackId;
    private List<string> _nominatingPlayerIds;
    private List<string> _upVotes;
    private List<string> _downVotes;

    public List<string> NominatingPlayerIds
    {
      get { return _nominatingPlayerIds ?? (_nominatingPlayerIds = new List<string>()); }
      set { _nominatingPlayerIds = value; }
    }

    public List<string> UpVotes
    {
      get { return _upVotes ?? (_upVotes = new List<string>()); }
      set { _upVotes = value; }
    }
    public List<string> DownVotes
    {
      get { return _downVotes ?? (_downVotes = new List<string>()); }
      set { _downVotes = value; }
    }
  }
}
