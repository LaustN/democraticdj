var Game = {
  SearchSelectionTimeout: null,
  SearchDelayTimeout: null,
  Search: function (event) {
    if (Game.SearchDelayTimeout!=null) {
      clearTimeout(Game.SearchDelayTimeout);
      Game.SearchDelayTimeout = null;
    }

    Game.SearchDelayTimeout = setTimeout(function() {
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
    },500);
  },
  SearchResultHandler: function (data) {
    console.log(data);
    var result = Game.RenderTracklist(data);
    $(".search-result-js").html(result);

  },

  RenderTracklist: function(data) {
    if (data && data.tracks && data.tracks.items) {
      var renderBuffer = [];
      $.each(data.tracks.items, function (index, track) {
        renderBuffer.push("<div class=\"found-track\" data-trackid=\"" + track.id + "\">");

        renderBuffer.push("<div class=\"track-name\">" + track.name + "</div>");

        if (track.artists) {
          renderBuffer.push("<div class=\"track-artist\">" + track.artists[0].name + "</div>");
        }

        if (track.album && track.album.images && track.album.images.length > 0) {
          renderBuffer.push("<div class=\"track-album\">" + track.album.name + "</div>");
          renderBuffer.push("<div class=\"track-image\"><img src=\"" + track.album.images[0].url + "\"></div>");
        }

        renderBuffer.push("</div>");

      });

      return renderBuffer.join("");
    }
    return "";
  },

  SearchResultSelection: function (event) {
    if (Game.SearchSelectionTimeout != null) {
      clearTimeout(Game.SearchSelectionTimeout);
      Game.SearchSelectionTimeout = null;
    }

    Game.SearchSelectionTimeout = setTimeout(function () {
      Game.SearchSelectionTimeout = null;

      var clickedTrackId = $(event.target).closest(".found-track").data("trackid");
      console.log(clickedTrackId);

      $.ajax({
        url: "/api/select",
        contentType: "application/json",
        data: JSON.stringify({
          trackid: clickedTrackId,
          gameid: $(document.forms[0]).data("gameid")
        }),
        method: "POST",
        success: Game.SearchResultHandler
      }
      );
    }, 500);
  },
  Init: function () {

    $(".search-box-js").keyup(Game.Search);
    $(".search-result-js").click(Game.SearchResultSelection);
  }


};

$(document).ready(Game.Init);