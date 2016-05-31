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
    if (data && data.tracks && data.tracks.items) {
      var result = Game.RenderTracklist(data.tracks.items);
      $(".search-result-js").html(result);
    }
  },

  RenderTracklist: function (tracks) {
    if (tracks) {
      var renderBuffer = [];
      $.each(tracks, function (index, track) {
        renderBuffer.push("<div class=\"track\" data-trackid=\"" + track.id + "\">");

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
    var clickedTrackId = $(event.target).closest(".track").data("trackid");
    console.log(clickedTrackId);

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

  RenderLists: function (data) {

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
    if (data && data.tracks) {
      var result = Game.RenderTracklist(data.tracks);
      $(".nominees-list-js").html(result);
    }
  },
  UpdatePlayerList: function (data) {
    if (data && data.tracks) {
      var result = Game.RenderTracklist(data.tracks);
      $(".player-list-js").html(result);
    }
  },

  UpdateWinnersList: function (data) {
    if (data && data.tracks) {
      var result = Game.RenderTracklist(data.tracks);
      $(".winners-list-js").html(result);
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