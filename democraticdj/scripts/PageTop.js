PageTop = {
  ToggleBurger: function() {
    $(".js-burger-target").toggleClass("hidden");
  },
  Init: function () {
    $(".js-burger").click(PageTop.ToggleBurger);
  }  
}

$(document).ready(PageTop.Init);
