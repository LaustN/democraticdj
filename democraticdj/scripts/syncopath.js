var Syncopath = {
  init: function() {
    $(".wait").click(Syncopath.wait);
    $(".poke").click(Syncopath.poke);
  },
  wait: function(event) {
    $.ajax("/api", {
      dataType: "json",
      success: Syncopath.callback
        
      });
  },
  poke: function(event) {
    $.ajax("/api", {
      dataType: "json",
      method: "POST",
      success: Syncopath.callback

    });

  },
  callback: function(data) {
    $(".target").html(data);
  },
  
}

$(document).ready(function() {
  Syncopath.init();
});