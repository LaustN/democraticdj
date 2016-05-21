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
          if (existingNominee != null)
          {
            existingNominee.NominatingPlayerIds.Add(player.UserId);
          }
          else
          {
            game.Nominees.Add(new Nominee
            {
              TrackId = trackId,
              NominatingPlayerIds = new List<string> { player.UserId }
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
      Dictionary<string, List<string>> votesTally = game.Nominees.ToDictionary(nominee => nominee.TrackId, nominee => new List<string>( nominee.NominatingPlayerIds));

      foreach (Vote vote in game.Votes)
      {
        votesTally[vote.TrackId].Add(vote.PlayerId);
      }

      string bestTrackId = string.Empty;
      int bestTrackVotes = 0;
      foreach (var trackId in votesTally.Keys)
      {
        if (votesTally[trackId].Count > bestTrackVotes)
        {
          bestTrackVotes = votesTally[trackId].Count;
          bestTrackId = trackId;
        }
      }

      //add track to game history with playerIds + votes
      var winner = new Winner
      {
        SelectingPlayerIds = game.Nominees.First(nominee => nominee.TrackId == bestTrackId).NominatingPlayerIds,
        TrackId = bestTrackId
      };
      game.PreviousWinners.Add(winner);

      //add that track to the playlist
      //TODO: call service

      //add all other tracks back onto the end of their nominators lists
      foreach (Nominee nominee in game.Nominees)
      {
        if (nominee.TrackId == bestTrackId)
          continue;
        foreach (var nominatingPlayerId in nominee.NominatingPlayerIds)
        {
          game.Players.First(player=> player.UserId == nominatingPlayerId).SelectedTracks.Add(nominee.TrackId);
        }
      }

      //award points for selected tracks
      foreach (var nominee in game.Nominees)
      {
        var currentNominee = nominee;

        var playersThatNominatedThisNominee =
          game.Players.Where(player => currentNominee.NominatingPlayerIds.Contains(player.UserId));

        foreach (Player player in playersThatNominatedThisNominee)
        {
          player.Points += votesTally[nominee.TrackId].Count;
        }
      }
    }

    public static void PlaceVote(Model.Game game, string playerId, string trackId)
    {
      var trackToUpvote = game.Nominees.FirstOrDefault(nomineeItem => nomineeItem.TrackId == trackId);
      if (trackToUpvote != null)
      {
        //ensure that vote is not for track nominated by player
        if (trackToUpvote.NominatingPlayerIds.Contains(playerId))
        {
          return;
        }

        //remove previous votes by same player, if any
        game.Votes.RemoveAll(vote => vote.PlayerId == playerId);

        //add vote to matching track
        game.Votes.Add(new Vote { PlayerId = playerId, TrackId = trackId });
      }


    }

    public static void SelectTrack(Model.Game game, string userId, string trackId)
    {
      Player selectingPlayer = game.Players.FirstOrDefault(player => player.UserId == userId);

      if (selectingPlayer == null)
      {
        selectingPlayer = new Player { UserId = userId };
        game.Players.Add(selectingPlayer);
      }

      if (selectingPlayer.SelectedTracks.Contains(trackId))
      {
        selectingPlayer.SelectedTracks.Remove(trackId);
      }
      selectingPlayer.SelectedTracks.Insert(0, trackId);
    }
  }
}