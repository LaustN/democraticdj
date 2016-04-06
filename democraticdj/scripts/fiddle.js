var init = function() {
// find template and compile it
  var templateSource = document.getElementById('results-template').innerHTML,
    template = Handlebars.compile(templateSource),
    resultsPlaceholder = document.getElementById('results'),
    playingCssClass = 'playing',
    audioObject = null;

  var fetchTracks = function(albumId, callback) {
    $.ajax({
      url: 'https://api.spotify.com/v1/albums/' + albumId,
      success: function(response) {
        callback(response);
      }
    });
  };

  var searchAlbums = function(query) {
    $.ajax({
      url: 'https://api.spotify.com/v1/search',
      data: {
        q: 'artist:' + query,
        type: 'album',
        market: "US"
      },
      success: function(response) {
        resultsPlaceholder.innerHTML = template(response);
      }
    });
  };

  results.addEventListener('click', function(e) {
    var target = e.target;
    if (target !== null && target.classList.contains('cover')) {
      if (target.classList.contains(playingCssClass)) {
        audioObject.pause();
      } else {
        if (audioObject) {
          audioObject.pause();
        }
        fetchTracks(target.getAttribute('data-album-id'), function (data) {
          console.log("Search results", data);
          var targetUrl = "https://open.spotify.com/track/" + data.tracks.items[0].id;
          

          var openedWindow = window.open(targetUrl, "SpotifyPlayWindow", "width= 640, height= 480, left=0, top=0, resizable=yes, toolbar=no, location=no, directories=no, status=no, menubar=no, scrollbars=yes, resizable=no, copyhistory=no").blur();
          window.focus();
          return;
          "https://open.spotify.com/track/" + data.tracks.items[0].id 

          audioObject = new Audio(data.tracks.items[0].preview_url);
          audioObject.play();
          target.classList.add(playingCssClass);
          audioObject.addEventListener('ended', function() {
            target.classList.remove(playingCssClass);
          });
          audioObject.addEventListener('pause', function() {
            target.classList.remove(playingCssClass);
          });
        });
      }
    }
  });

  var timeoutHandle = 0;
  $("#search-box").keypress(function (event) {
    if (timeoutHandle) {
      clearTimeout(timeoutHandle);
    }
    var searchTerm = $(event.target).val();
    timeoutHandle = setTimeout(function() {
      searchAlbums(searchTerm);
    }, 500);
    
  });

  searchAlbums('Leonard Cohen');
}
$(document).ready(init);