using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Democraticdj.Model
{
  public class Game
  {
    public string SpotifyUserId;
    public string SpotifyUserName;
    public string SpotifyPlaylistId;


  }

  public class Player
  {
    public string PlayerId;

    public List<string> SelectedTracks; //just a set of spotify track IDs

    public int Points;

  }
}