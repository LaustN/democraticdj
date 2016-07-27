using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Democraticdj.Model;
using Democraticdj.Services;

namespace Democraticdj.Logic
{
  public class GameLogic
  {
    public static object GameLogicLock = new object();

    public static void CreateBallot(string gameId)
    {
      lock (GameLogicLock)
      {
        Model.Game game = StateManager.Db.GetGame(gameId);
        if (game == null)
          return;

        game.BallotCreationTime = DateTime.UtcNow;

        //resetting votes when a new ballot is created
        game.MinimumVotesCastTime = null;
        game.Votes = new List<Vote>();

        game.Nominees = new List<Nominee>();
        foreach (Player player in game.Players.Where(player => player.SelectedTracks.Any()))
        {
          var selectedTrack = player.SelectedTracks[0];
          player.SelectedTracks.Remove(selectedTrack);
          Nominee nominee = game.Nominees.FirstOrDefault(existingNominee => existingNominee.TrackId == selectedTrack);
          if (nominee != null)
          {
            nominee.NominatingPlayerIds.Add(player.UserId);
          }
          else
          {
            game.Nominees.Add(new Nominee
            {
              TrackId = selectedTrack,
              NominatingPlayerIds = new List<string> { player.UserId }
            });
          }
        }

        Random random = new Random();
        for (int shuffleCount = 0; shuffleCount < game.Nominees.Count; shuffleCount++)
        {
          int pickedIndex = random.Next(game.Nominees.Count);
          var pickedNominee = game.Nominees[pickedIndex];
          game.Nominees.RemoveAt(pickedIndex);
          game.Nominees.Add(pickedNominee);
        }
        StateManager.Db.SaveGame(game);
      }
    }

    public static void ResolveRound(string gameId)
    {
      lock (GameLogicLock)
      {
        Model.Game game = StateManager.Db.GetGame(gameId);
        if (game == null)
          return;

        //find the track with the most votes
        Dictionary<string, List<string>> votesTally = game.Nominees.ToDictionary(nominee => nominee.TrackId,
          nominee => new List<string>(nominee.NominatingPlayerIds));

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
        SpotifyServices.AppendTrackToPlaylist(game, bestTrackId);

        //add all other tracks back onto the end of their nominators lists
        foreach (Nominee nominee in game.Nominees)
        {
          if (nominee.TrackId == bestTrackId)
            continue;
          foreach (var nominatingPlayerId in nominee.NominatingPlayerIds)
          {
            game.Players.First(player => player.UserId == nominatingPlayerId).SelectedTracks.Add(nominee.TrackId);
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

        game.Nominees = null;
        game.Votes = null;
        StateManager.Db.SaveGame(game);
      }
    }

    public static bool PlaceVote(string gameId, string playerId, string trackId)
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
          int oldVoteCount = game.Votes.Count;

          //remove previous votes by same player, if any
          game.Votes.RemoveAll(vote => vote.PlayerId == playerId);

          //add vote to matching track
          game.Votes.Add(new Vote { PlayerId = playerId, TrackId = trackId });

          if (oldVoteCount < game.MinimumVotes && game.Votes.Count == game.MinimumVotes)
          {
            game.MinimumVotesCastTime = DateTime.UtcNow;
          }
          StateManager.Db.SaveGame(game);
          return UpdateGameState(gameId);
        }

        return false;
      }
    }

    private static readonly object _selectTrackLock = new object();

    public static bool SelectTrack(string gameId, string userId, string trackId)
    {
      lock (_selectTrackLock)
      {
        Model.Game game = StateManager.Db.GetGame(gameId);
        if (game == null)
          return false;


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

        StateManager.Db.SaveGame(game);

        return UpdateGameState(gameId);
      }
    }

    private static readonly object _updateGameStateLock = new object();
    private static readonly Random LocalRandom = new Random();
    public static bool UpdateGameState(string gameId)
    {
      lock (_updateGameStateLock)
      {
        Model.Game game = StateManager.Db.GetGame(gameId);
        if (game == null)
          return false;

        long updateStartTicks = DateTime.UtcNow.Ticks;
        int randomizedIdOfThisThread = LocalRandom.Next();

        if (game.GameUpdateLock != null && (game.GameUpdateLock.UpdatingStartedTicks + 10000000 * 5) > updateStartTicks)
        {
          return false;
        }
        
        game.GameUpdateLock = new GameUpdateLock
        {
          UpdatingStartedTicks = updateStartTicks,
          UpdatingThreadRandomizedId = randomizedIdOfThisThread
        };

        StateManager.Db.SaveGame(game);

        Model.Game verificationGame = StateManager.Db.GetGame(gameId);

        if (verificationGame.GameUpdateLock == null ||
            verificationGame.GameUpdateLock.UpdatingThreadRandomizedId != randomizedIdOfThisThread)
        {
          return false;
        }


        bool result = false;
        //if enough votes have been cast and enough time has passed, resolve winner
        if (game.Votes.Count >= game.MinimumVotes
            && game.MinimumVotesCastTime.HasValue
            && (DateTime.UtcNow - game.MinimumVotesCastTime.Value).TotalSeconds >= game.VoteClosingDelay
          )
        {
          ResolveRound(gameId);
          result = true;
        }

        //if no ballot exists and 2 tracks can be chosen from, make a new ballot
        if (game.Players.Count(player => player.SelectedTracks.Any()) >= 2 && game.Nominees.Count == 0)
        {
          CreateBallot(gameId);
          result = true;

        }

        if (result)
        {
          game = StateManager.Db.GetGame(gameId);
          game.GameUpdateLock = null;
          StateManager.Db.SaveGame(game);
        }

        StateManager.UpdateGameTick(game);
        return result;
      }
    }

  }
}