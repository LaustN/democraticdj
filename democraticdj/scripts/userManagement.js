var UserManagement = {
  VerifyEmail: function (event) {
    var $clickedEmail = $(event.target).closest(".email");
    var clickedEmailValue = $clickedEmail.data("address");

    if ($clickedEmail.hasClass("notverified")) {
      $.ajax({
        url: "/api/verifyemail",
        contentType: "application/json",
        data: JSON.stringify({
          email: clickedEmailValue
        }),
        method: "POST",
        success: function(data) {
          $clickedEmail.removeClass("notverified").addClass("verifying");
        }
      });

    }
  },



  Init: function() {
    $(".emails").click(UserManagement.VerifyEmail);
  }

};

$(document).ready(UserManagement.Init);