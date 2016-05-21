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

    public List<string> NominatingPlayerIds
    {
      get { return _nominatingPlayerIds ?? (_nominatingPlayerIds = new List<string>()); }
      set { _nominatingPlayerIds = value; }
    }

  }
}
