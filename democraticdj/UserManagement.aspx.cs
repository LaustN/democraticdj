using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Democraticdj.Model;
using Democraticdj.Services;

using Reachmail.Easysmtp;
using Reachmail.Easysmtp.Post.Request;

namespace Democraticdj
{
  public partial class UserManagement : System.Web.UI.Page
  {
    protected void Page_Load(object sender, EventArgs e)
    {
      var authResponseCode = Request.QueryString["code"];
      var responseState = Request.QueryString["state"];
      if (!string.IsNullOrEmpty(authResponseCode))
      {
        var tokens = SpotifyServices.ProcessAuthCode(authResponseCode, responseState);
        if (tokens != null)
        {
          tokens.ReceivedTime = DateTime.UtcNow;

          Model.Spotify.SpotifyUser spotifyUser = SpotifyServices.GetAuthenticatedUser(tokens);
          var spotifyAuthenticatedUser = StateManager.Db.FindExistingUser(spotifyUser);
          if (spotifyAuthenticatedUser != null)
          {
            using (var session = StateManager.CurrentSession)
            {
              session.UserId = spotifyAuthenticatedUser.UserId;
            }
          }
          using (User currentUser = StateManager.CurrentUser)
          {
            currentUser.SpotifyAuthTokens = tokens;
            currentUser.SpotifyUser = spotifyUser;
          }
          if (!string.IsNullOrWhiteSpace(responseState))
          {
            Response.Redirect(responseState);
          }
          Response.Redirect("UserManagement.aspx");
        }
      }
      using (User user = StateManager.CurrentUser)
      {
        if (user.IsLoggedIn)
        {
          KnownUser.Visible = true;
          UnknownUser.Visible = false;

          var verifyingEmailId = Request.QueryString["emailverification"];
          if (!string.IsNullOrWhiteSpace(verifyingEmailId))
          {
            var userEmailToVerify = user.Emails.FirstOrDefault(item => item.PendingVerificationId == verifyingEmailId);
            if (userEmailToVerify != null)
            {
              userEmailToVerify.IsVerified = true;
              Response.Redirect(SpotifyServices.RedirectUrl,false);
              return;
            }
          }

          var useIcon = Request.QueryString["useIcon"];
          if (!string.IsNullOrEmpty(useIcon))
          {
            int parsedIconIdndex;
            if (int.TryParse(useIcon, out parsedIconIdndex) && GravatarOptions.Count()>parsedIconIdndex)
            {
              user.AvatarUrl = GravatarOptions.Skip(parsedIconIdndex).First().Url;
              Response.Redirect(SpotifyServices.RedirectUrl, false);
              return;
            }

          }

          if (IsPostBack)
          {
            if (!string.IsNullOrWhiteSpace(NameBox.Value) && NameBox.Value != user.DisplayName)
            {
              user.DisplayName = NameBox.Value;
            }

            var newPassword = PasswordUpdate.Value;
            var repeatPassword = PasswordRepeat.Value;

            if (!string.IsNullOrWhiteSpace(newPassword) && !string.IsNullOrWhiteSpace(repeatPassword) && newPassword == repeatPassword)
            {
              user.Password = newPassword;
            }
          }
          else
          {
            if (string.IsNullOrWhiteSpace(user.UserName))
            {
              UserNameAsLabelWrapper.Visible = false;
            }
            else
            {
              UserNameAsLabel.Text = user.UserName;
            }
            NameBox.Value = user.DisplayName;
          }

          if (user.SpotifyAuthTokens != null && !string.IsNullOrWhiteSpace(user.SpotifyAuthTokens.AccessToken))
          {
            SpotifyAuthLink.Visible = false;
            SpotifyInfo.Visible = true;
          }
          else
          {
            SpotifyAuthLink.Visible = true;
            SpotifyInfo.Visible = false;
          }

        }
        else
        {
          KnownUser.Visible = false;
          UnknownUser.Visible = true;

          var existingUserEmailOrName = Request.Form["existingUserEmailOrUsername"];
          var existingUserPassword = Request.Form["existingUserPassword"];

          if (!string.IsNullOrWhiteSpace(existingUserEmailOrName) && !string.IsNullOrWhiteSpace(existingUserPassword))
          {
            var foundUser = StateManager.Db.FindExistingUser(existingUserEmailOrName);

            if (foundUser != null && foundUser.Password == existingUserPassword)
            {
              using (var session = StateManager.CurrentSession)
              {
                session.UserId = foundUser.UserId;
              }
              Response.Redirect("/");
            }
            else
            {
              //TODO: print error stating that login failed
            }
          }

          var newUserEmail = Request.Form["newUserEmail"];
          var newUserName = Request.Form["newUserName"];
          var newPassword = Request.Form["newPassword"];
          if (!string.IsNullOrWhiteSpace(newUserEmail) && !string.IsNullOrWhiteSpace(newUserName) && !string.IsNullOrWhiteSpace(newPassword))
          {
            var existingUser =
              StateManager.Db.FindExistingUser(newUserEmail) ??
              StateManager.Db.FindExistingUser(newUserName);
            if (existingUser != null)
            {
              //TODO: say that the email or username is taken
            }
            else
            {
              user.UserName = newUserName;
              user.Password = newPassword;
              user.Emails.Add(new UserEmail { Address = newUserEmail });
            }
          }
        }

      }
      DataBind();
    }

    public IEnumerable<Model.Game> ExistingGames
    {
      get
      {
        using (var user = StateManager.CurrentUser)
        {
          return StateManager.Db.FindGamesStartedByUser(user.UserId);
        }
      }
    }

    public IEnumerable<UserEmail> Emails
    {
      get
      {
        using (var user = StateManager.CurrentUser)
        {
          user.ShouldAutoSave = false;
          return user.Emails;
        }

      }
    }

    public IEnumerable<IconOption> GravatarOptions
    {
      get
      {
        if (!Emails.Any(email => email.IsVerified))
          yield break;

        int iconIdex = 0;
        yield return
          new IconOption
          {
            Url = "/graphics/mediaplayer.png",
            Index = iconIdex++
          };

        var hashEngine = MD5.Create();
        foreach (var verifiedEmail in Emails.Where(email => email.IsVerified))
        {
          byte[] data = hashEngine.ComputeHash(Encoding.UTF8.GetBytes(verifiedEmail.Address));
          StringBuilder sBuilder = new StringBuilder();
          for (int i = 0; i < data.Length; i++)
          {
            sBuilder.Append(data[i].ToString("x2"));
          }
          string hash = sBuilder.ToString();
          yield return new IconOption
          {
            Url = "https://www.gravatar.com/avatar/" + hash + "?d=retro",
            Index = iconIdex++
          };
        }
      }
    }
  }
}