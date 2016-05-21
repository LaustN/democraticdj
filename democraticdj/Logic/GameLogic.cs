using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Democraticdj.Model;

namespace Democraticdj.Logic
{
  public class GameLogic
  {
    public static void CreateBallot(Model.Game game)
    {
      game.Nominees = null; //resets list to empty, since first get() creates new list

      foreach (var player in game.Players)
      {
        if (player.SelectedTracks.Any())
        {
          var trackId = player.SelectedTracks.First();
          var existingNominee =
            game.Nominees.FirstOrDefault(nominee => nominee.TrackId == trackId);
          if (existingNominee!=null)
          {
            existingNominee.NominatingPlayerIds.Add(player.UserId);
          }
          else
          {
            game.Nominees.Add(new Nominee
            {
              TrackId = trackId,
              NominatingPlayerIds = new List<string>{player.UserId}
            });
          }
          player.SelectedTracks.RemoveAt(0);
        }
      }
    }

    public static void ResolveRound(Model.Game game)
    {
      //TODO: implement described logic

      //find the track with the most votes
      //add that track to the playlist
      //add track to game history with playerIds + votes
      //add all other tracks back onto the end of their nominators lists
      //award points for selected tracks
      //  1 point for each vote + 1 point for each nominator
    }

    public static void PlaceVote(Model.Game game, string playerId, string trackId)
    {
      //TODO: implement described logic 
      //ensure that vote is not for track nominated by player
      //remove previous votes by same player, if any
      //add vote to matching track
    }

    public static void SelectTrack(Model.Game game, string userId, string trackId)
    {
      Player selectingPlayer = game.Players.FirstOrDefault(player => player.UserId == userId);

      if (selectingPlayer == null)
      {
        selectingPlayer = new Player {UserId = userId};
        game.Players.Add(selectingPlayer);
      }

      if (selectingPlayer.SelectedTracks.Contains(trackId))
      {
        selectingPlayer.SelectedTracks.Remove(trackId);
      }
      selectingPlayer.SelectedTracks.Insert(0,trackId);
    }
  }
}