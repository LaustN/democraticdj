﻿var Game = {
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

  RenderNominees: function (nominees) {
    var $nomineesList = $(".nominees-list-js");
    var $sharethisgame = $(".sharethisgame");

    if (nominees && nominees.length > 0) {
      $sharethisgame.addClass("hidden");
      var renderBuffer = [];
      $.each(nominees,
        function (index, nominee) {

          var $preExistingNominee = $nomineesList.find(".nominee[data-nomineeid=\"" + nominee.id + "\"]");

          if ($preExistingNominee.length === 0) {
            renderBuffer.push("<div class=\"nominee");
            if (nominee.preview_url) {
              renderBuffer.push(" haspreview");
            }
            else {
              renderBuffer.push(" nopreview");
            }
            renderBuffer.push("\" data-nomineeid=\"" + nominee.id + "\">");

            if (nominee.preview_url) {
              renderBuffer.push("<audio><source src=\"" + nominee.preview_url + "\" type=\"audio/mpeg\" /></audio>");
            }

            console.log(nominee);

            renderBuffer.push("<div class=\"nominee-button js-no\" >No</div>");

            if (nominee.album && nominee.album.images && nominee.album.images.length > 0) {
              renderBuffer.push("<div class=\"nominee-image\"><img src=\"" + nominee.album.images[0].url + "\"></div>");
            }
            renderBuffer.push("<div class=\"nominee-button js-yes\" >Yes</div>");

            renderBuffer.push("<div class=\"nominee-label\">" + nominee.name + "</div>");

            if (nominee.artists) {
              renderBuffer.push("<div class=\"nominee-label\">" + nominee.artists[0].name + "</div>");
            }
            renderBuffer.push("</div>");
          }
        });

      var renderedHtml = renderBuffer.join("");
      $nomineesList.append(renderedHtml);
    } else {
      $sharethisgame.removeClass("hidden");
    }
  },

  PlaceVote: function (event) {
    var $clickedElement = $(event.target);
    var $clickedNominee = $clickedElement.closest(".nominee");
    var clickedNomineeId = $clickedNominee.data("nomineeid");

    var $clickedButton = $clickedElement.closest(".nominee-button");
    var voteCast = false;

    if ($clickedButton.length == 1) {
      voteCast = true;
      $clickedNominee.css("height", $clickedNominee.height() + "px");
      setTimeout(function () {
        $clickedNominee.css("height", "0");
      }, 1);

      var isUpvote = $clickedButton.hasClass("js-yes");

      $.ajax({
        url: "/api/vote",
        contentType: "application/json",
        data: JSON.stringify({
          trackid: clickedNomineeId,
          gameid: $(document.forms[0]).data("gameid"),
          isupvote: isUpvote
        }),
        method: "POST",
        success: Game.RefreshGameData
      }
      );
    }
    var $audio = $clickedNominee.find("audio");
    if ($audio.length > 0) {
      var audioTag = $audio[0];
      if (voteCast) {
        audioTag.pause();
        $clickedNominee.removeClass("playing");
      } else {
        if (audioTag.paused) {
          audioTag.play();
          $clickedNominee.addClass("playing");

          audioTag.addEventListener('ended', function () {
            $clickedNominee.removeClass("playing");
          });
        } else {
          audioTag.pause();
          audioTag.load();
          $clickedNominee.removeClass("playing");
        }
      }
    }
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
          $clickedTrack.addClass("ok");
          $clickedTrack.removeClass("spinner");
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

  RenderLists: function (data) {

    Game.GameState = data;

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
    if (data && data.tracks) {
      Game.RenderNominees(data.tracks);
    } else {
      Game.RenderNominees(null);
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

  PlayerListClick: function (event) {
    var $clickedElement = $(event.target);
    var $clickedTrack = $clickedElement.closest(".track");
    var clickedTrackId = $clickedTrack.data("trackid");

    var $button = $clickedElement.closest(".js-button");
    if ($button.length == 1) {
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

        $clickedTrack.addClass("spinner");
      }

    }


    var $buttonsHolder = $clickedTrack.find(".js-buttons");
    if ($buttonsHolder.length == 0) {
      $(".player-list-js").find(".js-buttons").remove();

      //insert buttons control
      $clickedTrack
        .append("<div class='js-buttons'><div class='js-button remove'><img src='/graphics/cross.svg' alt='Remove from the list' title='Remove from the list' /></div></div>");
    } else {
      $(".player-list-js").find(".js-buttons").remove();

      //remove buttons holder after each click
      $buttonsHolder.remove();
    }

  },

  Players: {},

  RenderLeaderBoard: function () {
    if (Game.GameState && Game.GameState.Scores && Game.GameState.Scores.length > 0) {
      var newPlayers = [];
      $.each(Game.GameState.Scores, function (index, item) {
        if (!Game.Players[item.PlayerId]) {
          newPlayers.push(item.PlayerId);
        }
      });
      if (newPlayers.length > 0) {
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

  GetUsersCallback: function (data) {
    $.each(data, function (index, item) {
      Game.Players[item.UserId] = item;
    });
    Game.RenderLeaderBoard();
  },
  PreventPostback: function (event) {
    if (event.keyCode == 13) {
      event.preventDefault();
      return false;
    }
    return true;
  },

  InitShareLinks: function () {
    var location = window.location.href;
    var smsLink = "sms:&body=" + encodeURIComponent("Build a playlist with me at " + location);
    var mailLink = "mailto:?body=" +
      encodeURIComponent("Build a playlist with me at " + location) +
      "&subject=" +
      encodeURIComponent("Build a playlist with me at " + location);
    $(".sharethisgame .shareaction.mail a").attr("href", mailLink);
    $(".sharethisgame .shareaction.sms a").attr("href", smsLink);
  },

  LoadUserPlaylists: function () {
    $.ajax({
      url: "/api/currentuserplaylists",
      contentType: "application/json",
      method: "GET",
      success: Game.LoadUserPlaylistsHandler
    });

  },

  LoadUserPlaylistsHandler: function (data) {
    var $myplaylists = $(".myplaylists");
    if (data && data.length > 0) {
      var optionTags = [];

      var lists = [];

      $.each(data, function(index, playlist) {
        optionTags.push(
          "<option value='" +
          playlist.id +
          "'>" +
          playlist.name +
          "</option>"
        );

      });

      $myplaylists.html("<select id='existing-playlist-select'><option>Choose tracks from your playlists</option>" + optionTags.join("") + "</select>" + lists.join(""));

      $("#existing-playlist-select").change(Game.ChangeExistingPlaylistSelection);
    } else {
      $myplaylists.remove();
    }
  },

  ChangeExistingPlaylistSelection: function (event) {
    var selectedValue = $(event.target).val();
    $(".existing-playlist").addClass("hidden");
    var $existingList = $("#" + selectedValue);
    if ($existingList.length>0) {
      $existingList.removeClass("hidden");
    }
    else {
      $.ajax({
        url: "/api/tracksfromcurrentuserplaylist?listId=" + selectedValue,
        contentType: "application/json",
        method: "GET",
        success: Game.LoadTracksFromCurrentUserPlaylist
      });

    }
  },

  LoadTracksFromCurrentUserPlaylist: function (data) {
    var list = [];

    list.push("<div class='existing-playlist' id='" + data.playlistId + "'>");
    list.push(Game.RenderTracklist(data.tracks));
    list.push("</div>");

    var $myplaylists = $(".myplaylists");

    $myplaylists.append(list.join(""));

  },

  Init: function () {

    $(".search-box-js").keyup(Game.Search);
    $(".search-box-js").keydown(Game.PreventPostback);
    $(".search-result-js").click(Game.SearchResultSelection);
    $(".myplaylists").click(Game.SearchResultSelection);
    $(".nominees-list-js").click(Game.PlaceVote);
    $(".player-list-js").click(Game.PlayerListClick);

    Game.InitShareLinks();
    Game.RefreshGameData();
    Game.AutoRefresh();
    Game.LoadUserPlaylists();
  }


};

$(document).ready(Game.Init);