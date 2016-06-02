var Game = {
  SearchSelectionTimeout: null,
  SearchDelayTimeout: null,
  Search: function (event) {
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

      var clickedTrackId = $(event.target).closest(".track").data("trackid");

      $.ajax({
        url: "/api/select",
        contentType: "application/json",
        data: JSON.stringify({
          trackid: clickedTrackId,
          gameid: $(document.forms[0]).data("gameid")
        }),
        method: "POST",
        success: Game.RefreshGameData
      }
      );
      $(".search-result-js").html("");
      $(".search-box-js").val("");

    }, 500);
  },

  AutoRefresh: function () {
    $.ajax({
      url: "/api/game/isupdated?gameid=" + $(document.forms[0]).data("gameid"),
      method: "GET",
      success: Game.AutoRefreshCallback
    });
  },
  AutoRefreshCallback: function (data) {
    if (data) {
      Game.RefreshGameData();
    }
    Game.AutoRefresh();
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
    $(".voting-countdown-js").html("" + Game.GameState.SecondsUntillVoteCloses + "seconds");
    Game.GameState.SecondsUntillVoteCloses--;
    if (Game.GameState.SecondsUntillVoteCloses<1) {
      clearInterval(Game.CountDownInterval);
      Game.RefreshGameData();
    }
  },

  RenderLists: function (data) {

    Game.GameState = data;

    if (Game.GameState.SecondsUntillVoteCloses > 0) {
      if (Game.CountDownInterval) {
        clearInterval(Game.CountDownInterval);
      }
      Game.Countdown();
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

  Init: function () {

    $(".search-box-js").keyup(Game.Search);
    $(".search-result-js").click(Game.SearchResultSelection);
    $(".nominees-list-js").click(Game.PlaceVote);

    Game.RefreshGameData();
    Game.AutoRefresh();
  }


};

$(document).ready(Game.Init);