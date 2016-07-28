var Game = {
  SearchSelectionTimeout: null,
  SearchDelayTimeout: null,
  Search: function (event) {
    event.preventDefault();
    if (Game.SearchDelayTimeout != null) {
      clearTimeout(Game.SearchDelayTimeout);
      Game.SearchDelayTimeout = null;
    }

    Game.SearchDelayTimeout = setTimeout(function () {
      Game.SearchDelayTimeout = null;
      var val = $(event.target).val();
      console.log(val);

      $.ajax({
        url: "/api/search",
        contentType: "application/json",
        data: JSON.stringify({
          query: val,
          gameid: $(document.forms[0]).data("gameid")
        }),
        method: "POST",
        success: Game.SearchResultHandler
      }
      );
    }, 500);
  },
  SearchResultHandler: function (data) {
    var $searchresults = $(".search-result-js");
    if (data && data.tracks && data.tracks.items) {
      var result = Game.RenderTracklist(data.tracks.items);
      $searchresults.html(result);
    } else {
      $searchresults.html("Your search did not yield any tracks");
    }

  },

  RenderTracklist: function (tracks, idToClassMap) {
    if (tracks) {
      var renderBuffer = [];
      $.each(tracks, function (index, track) {
        renderBuffer.push("<div class=\"track ");
        if (idToClassMap && idToClassMap[track.id]) {
          renderBuffer.push(" ");
          renderBuffer.push(idToClassMap[track.id]);
        }
        renderBuffer.push("\" data-trackid=\"" + track.id + "\">");

        if (track.album && track.album.images && track.album.images.length > 0) {
          renderBuffer.push("<div class=\"track-image\"><img src=\"" + track.album.images[0].url + "\"></div>");
        }

        renderBuffer.push("<div class=\"track-name\">" + track.name + "</div>");

        if (track.album) {
          renderBuffer.push("<div class=\"track-album\">" + track.album.name + "</div>");
        }

        if (track.artists) {
          renderBuffer.push("<div class=\"track-artist\">" + track.artists[0].name + "</div>");
        }
        renderBuffer.push("</div>");

      });

      return renderBuffer.join("");
    }
    return "";
  },

  PlaceVote: function (event) {
    var $clickedTrack = $(event.target).closest(".track");
    var clickedTrackId = $clickedTrack.data("trackid");
    $clickedTrack.addClass("spinner");

    $.ajax({
      url: "/api/vote",
      contentType: "application/json",
      data: JSON.stringify({
        trackid: clickedTrackId,
        gameid: $(document.forms[0]).data("gameid")
      }),
      method: "POST",
      success: Game.RefreshGameData
    }
    );

  },

  SearchResultSelection: function (event) {
    if (Game.SearchSelectionTimeout != null) {
      clearTimeout(Game.SearchSelectionTimeout);
      Game.SearchSelectionTimeout = null;
    }

    Game.SearchSelectionTimeout = setTimeout(function () {
      Game.SearchSelectionTimeout = null;

      var $clickedTrack = $(event.target).closest(".track");
      var clickedTrackId = $clickedTrack.data("trackid");

      $clickedTrack.addClass("spinner");
      $.ajax({
        url: "/api/select",
        contentType: "application/json",
        data: JSON.stringify({
          trackid: clickedTrackId,
          gameid: $(document.forms[0]).data("gameid")
        }),
        method: "POST",
        success: function () {
            Game.RefreshGameData();
            $clickedTrack.remove();
          }
        }
      );

    }, 500);
  },

  AutoRefresh: function () {
    $.ajax({
      url: "/api/game/isupdated?gameid=" + $(document.forms[0]).data("gameid"),
      method: "GET",
      success: Game.AutoRefreshCallback,
      complete: Game.AutoRefresh
    });
  },
  AutoRefreshCallback: function (data) {
    Game.RefreshGameData();
  },

  RefreshGameData: function () {
    $.ajax({
      url: "/api/game?gameid=" + $(document.forms[0]).data("gameid"),
      method: "GET",
      success: Game.RenderLists
    });


  },

  GameState: {},

  CountDownInterval: 0,
  Countdown : function() {
    console.log(Game.GameState.SecondsUntillVoteCloses);
    var $votingCountdownHolder = $(".voting-countdown-js");
    $votingCountdownHolder.html("" + Game.GameState.SecondsUntillVoteCloses + " seconds untill voting closes");
    if (Game.GameState.SecondsUntillVoteCloses<0) {
      clearInterval(Game.CountDownInterval);
      $votingCountdownHolder.html("");
      $(".nominees-list-js").html("counting votes");

      setTimeout(Game.RefreshGameData, 500);
    }
    Game.GameState.SecondsUntillVoteCloses--;
  },

  RenderLists: function (data) {

    Game.GameState = data;

    if (Game.GameState.SecondsUntillVoteCloses > 0) {
      if (Game.CountDownInterval) {
        clearInterval(Game.CountDownInterval);
      }
      Game.CountDownInterval = setInterval(Game.Countdown,1000);
    }

    $.ajax({
      url: "/api/tracks",
      contentType: "application/json",
      data: JSON.stringify({
        tracks: data.Nominees,
        gameid: $(document.forms[0]).data("gameid")
      }),
      method: "POST",
      success: Game.UpdateNominees
    });

    $.ajax({
      url: "/api/tracks",
      contentType: "application/json",
      data: JSON.stringify({
        tracks: data.PlayerSelectionList,
        gameid: $(document.forms[0]).data("gameid")
      }),
      method: "POST",
      success: Game.UpdatePlayerList
    });

    $.ajax({
      url: "/api/tracks",
      contentType: "application/json",
      data: JSON.stringify({
        tracks: data.Winners,
        gameid: $(document.forms[0]).data("gameid")
      }),
      method: "POST",
      success: Game.UpdateWinnersList
    });

    Game.CurrentVote = data.CurrentVote;

    //render the winners list
    Game.RenderLeaderBoard();
    var $votesCastHolder = $(".votes-cast-js");
    if (data.VotesCastCount > 0) {
      $votesCastHolder.html("Votes cast: " + data.VotesCastCount);
    } else {
      $votesCastHolder.html("");
    }
  },

  CurrentVote: null,

  UpdateNominees: function (data) {
    var $nominees = $(".nominees-list-js");
    if (data && data.tracks) {

      var idToClassMap = {};
      if (Game.GameState.CurrentVote) {
        idToClassMap[Game.GameState.CurrentVote] = "current-vote";
      }
      if (Game.GameState.PlayersSelection) {
        idToClassMap[Game.GameState.PlayersSelection] = "players-selection";
      }

      var result = Game.RenderTracklist(data.tracks, idToClassMap);
      $nominees.html(result);
    } else {
      $nominees.html("once enough players have nominated a track, the list of nominees will render here");
    }
  },
  UpdatePlayerList: function (data) {

    var $playerList = $(".player-list-js"); 
    if (data && data.tracks) {
      var result = Game.RenderTracklist(data.tracks);
      $playerList.html(result);
    } else {
      $playerList.html("search for some track, then select some ");
    }
  },

  UpdateWinnersList: function (data) {
    var idToClassMap = {};

    if (Game.GameState.PlayersWinners) {
      $.each(Game.GameState.PlayersWinners, function (index, winner) {
        idToClassMap[winner] = "players-winner";
      });
    }

    var $winnersList = $(".winners-list-js"); 
    if (data && data.tracks) {
      var result = Game.RenderTracklist(data.tracks, idToClassMap);
      $winnersList.html(result);
    } else {
      $winnersList.html("the winners will be rendered here");
    }
  },

  PlayerListClick: function(event) {
    var $clickedElement = $(event.target);
    var $clickedTrack = $clickedElement.closest(".track");
    var clickedTrackId = $clickedTrack.data("trackid");

    var $button = $clickedElement.closest(".js-button");
    if($button.length == 1) {
      var commandExecuting = false;
      if ($button.hasClass("remove")) {
        $.ajax({
          url: "/api/unselect",
          contentType: "application/json",
          data: JSON.stringify({
            trackid: clickedTrackId,
            gameid: $(document.forms[0]).data("gameid")
          }),
          method: "POST",
          success: function () {
            Game.RefreshGameData();
          }
        }
        );

        commandExecuting = true;
      }

      if ($button.hasClass("top")) {
        $.ajax({
            url: "/api/select",
            contentType: "application/json",
            data: JSON.stringify({
              trackid: clickedTrackId,
              gameid: $(document.forms[0]).data("gameid")
            }),
            method: "POST",
            success: function () {
              Game.RefreshGameData();
            }
          }
        );

        commandExecuting = true;
      }

      if (commandExecuting) {
        //add spinner
        $clickedTrack.addClass("spinner");
      }
    }


    var $buttonsHolder = $clickedTrack.find(".js-buttons");
    if ($buttonsHolder.length == 0) {
      $(".player-list-js").find(".js-buttons").remove();

      //insert buttons control
      $clickedTrack
        .append("<div class='js-buttons'><div class='js-button top'><img src='/graphics/start.svg' alt='Move to the top of the list' title='Move to the top of the list' /></div><div class='js-button remove'><img src='/graphics/cross.svg' alt='Remove from the list' title='Remove from the list' /></div></div>");
    } else {
      $(".player-list-js").find(".js-buttons").remove();

      //remove buttons holder after each click
      $buttonsHolder.remove();
    }

  },

  Players: {},

  RenderLeaderBoard: function() {
    if (Game.GameState && Game.GameState.Scores && Game.GameState.Scores.length > 0) {
      var newPlayers = [];
      $.each(Game.GameState.Scores, function(index, item) {
        if (!Game.Players[item.PlayerId]) {
          newPlayers.push(item.PlayerId);
        }
      });
      if (newPlayers.length>0) {
        $.ajax({
          url: "/api/users",
          contentType: "application/json",
          data: JSON.stringify(newPlayers),
          method: "POST",
          success: Game.GetUsersCallback
        });
      } else {
        var $scores = $(".player-scores-js");
        var renderBuffer = [];
        //the actual rendering
        $.each(Game.GameState.Scores, function (index, item) {
          var currentPlayer = Game.Players[item.PlayerId];
          renderBuffer.push("<div class=\"player\">");

          renderBuffer.push("<div class=\"player-icon\">");
          renderBuffer.push("<img src=\"");
          if (currentPlayer.AvatarUrl && currentPlayer.AvatarUrl.length > 0) {
            renderBuffer.push(currentPlayer.AvatarUrl);
          } else {
            renderBuffer.push("/graphics/mediaplayer.png");
          }
          renderBuffer.push("\" />");
          renderBuffer.push("</div>");

          renderBuffer.push("<div class=\"player-name\">");
          renderBuffer.push(currentPlayer.Name);
          renderBuffer.push("</div>");


          renderBuffer.push("<div class=\"player-points\">");
          renderBuffer.push(item.Points);
          renderBuffer.push(" points</div>");

          renderBuffer.push("</div>");
        });
        $scores.html(renderBuffer.join(""));
      }
    }
  },

  GetUsersCallback: function(data) {
    $.each(data, function(index, item) {
      Game.Players[item.UserId] = item;
    });
    Game.RenderLeaderBoard();
  },
  PreventPostback: function(event) {
    if (event.keyCode == 13) {
      event.preventDefault();
      return false;
    }
    return true;
  },

  Init: function () {

    $(".search-box-js").keyup(Game.Search);
    $(".search-box-js").keydown(Game.PreventPostback);
    $(".search-result-js").click(Game.SearchResultSelection);
    $(".nominees-list-js").click(Game.PlaceVote);
    $(".player-list-js").click(Game.PlayerListClick);

    Game.RefreshGameData();
    Game.AutoRefresh();
  }


};

$(document).ready(Game.Init);