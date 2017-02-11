using System;
using System.Collections.Generic;
using System.Linq;
using Democraticdj.Model;
using Democraticdj.Services;
using Reachmail.Lists.Post.Request;

namespace Democraticdj.Logic
{
  public class GameLogic
  {
    public static object GameLogicLock = new object();

    public static bool PlaceVote(string gameId, string playerId, string trackId, bool isUpVote)
    {
      lock (GameLogicLock)
      {
        Model.Game game = StateManager.Db.GetGame(gameId);
        if (game == null)
          return false;


        var trackToUpvote = game.Nominees.FirstOrDefault(nomineeItem => nomineeItem.TrackId == trackId);
        if (trackToUpvote != null)
        {
          //ensure that vote is not for track nominated by player
          if (trackToUpvote.NominatingPlayerIds.Contains(playerId))
          {
            return false;
          }
          if (isUpVote && !trackToUpvote.UpVotes.Contains(playerId))
          {
            trackToUpvote.UpVotes.Add(playerId);
          }
          if (!isUpVote && !trackToUpvote.DownVotes.Contains(playerId))
          {
            trackToUpvote.DownVotes.Add(playerId);
          }
          game.GameStateUpdateTime = DateTime.UtcNow;

          StateManager.Db.SaveGame(game);
          UpdateSpotifyList(game);
        }

        return false;
      }
    }

    private static readonly object _selectTrackLock = new object();
    public static void SelectTrack(string gameId, string userId, string trackId, bool unselect = false)
    {
      lock (_selectTrackLock)
      {
        Model.Game game = StateManager.Db.GetGame(gameId);
        if (game == null)
          return;


        Player selectingPlayer = game.Players.FirstOrDefault(player => player.UserId == userId);

        if (selectingPlayer == null)
        {
          selectingPlayer = new Player { UserId = userId };
          game.Players.Add(selectingPlayer);
        }

        var existingNominee = game.Nominees.FirstOrDefault(nominee => nominee.TrackId == trackId);

        if (existingNominee != null)
        {
          if (unselect)
          {
            existingNominee.NominatingPlayerIds.Remove(userId);
            if (existingNominee.NominatingPlayerIds.Count == 0)
            {
              if (existingNominee.UpVotes.Count > 0)
              {
                existingNominee.NominatingPlayerIds.AddRange(existingNominee.UpVotes);
                existingNominee.UpVotes = null;
              }
              else
              {
                game.Nominees.Remove(existingNominee);
              }
            }
          }
          else
          {
            //selecting only allowed for tracks not currently voted on
            if (!existingNominee.UpVotes.Contains(userId) && !existingNominee.DownVotes.Contains(userId))
            {
              existingNominee.NominatingPlayerIds.Add(userId);
            }
          }
        }
        else
        {
          if (!unselect)
          {
            var newNominee = new Nominee { TrackId = trackId };
            newNominee.NominatingPlayerIds.Add(userId);
            game.Nominees.Add(newNominee);
          }
        }
        game.GameStateUpdateTime = DateTime.UtcNow;
        UpdateSpotifyList(game);

        StateManager.Db.SaveGame(game);
      }
    }

    private static void UpdateSpotifyList(Model.Game game)
    {
      List<string> trackIds = new List<string>();

      Dictionary<string, int> tracksCountPerPlayer = new Dictionary<string, int>();
      int maxTracksPerPlayer = (int)Math.Floor(100.0 / game.Players.Count);

      foreach (Nominee nominee in game.Nominees
                                      .Where(nominee => nominee.UpVotes.Count + nominee.NominatingPlayerIds.Count > nominee.DownVotes.Count)
                                      .OrderByDescending(
                                        nominee => nominee.UpVotes.Count + nominee.NominatingPlayerIds.Count - nominee.DownVotes.Count))
      {
        string nominatingPlayer = nominee.NominatingPlayerIds.FirstOrDefault();
        if (tracksCountPerPlayer.ContainsKey(nominatingPlayer))
        {
          if (tracksCountPerPlayer[nominatingPlayer] < maxTracksPerPlayer)
          {
            tracksCountPerPlayer[nominatingPlayer]++;
            trackIds.Add(nominee.TrackId);
          }
        }
        else
        {
          tracksCountPerPlayer[nominatingPlayer] = 1;
          trackIds.Add(nominee.TrackId);
        }
      }
      SpotifyServices.ReOrderPlaylist(game, trackIds.ToArray());

    }

  }
}