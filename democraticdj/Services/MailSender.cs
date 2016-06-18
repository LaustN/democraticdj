using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Reachmail.Easysmtp.Post.Request;

namespace Democraticdj.Services
{
  public class MailSender
  {
    public static bool Send(string subject, string body, string recipientAddress, bool isBodyHtml = false)
    {
      string reachMailAccountKey = "ReachMail.AccountKey";
      string reachMailUserName = "ReachMail.UserName";
      string reachMailPassword = "ReachMail.Password";

      var mailAccountKey = ConfigurationManager.AppSettings[reachMailAccountKey];
      var mailUserName = ConfigurationManager.AppSettings[reachMailUserName];
      var mailPassword = ConfigurationManager.AppSettings[reachMailPassword];

      var reachMailApiClient = Reachmail.Api.Create(mailAccountKey, mailUserName, mailPassword);
      var postRequest = new DeliveryRequest
      {
        FooterAddress = "noreply@democraticdj.apphb.com",
        FromAddress = "noreply@democraticdj.apphb.com",
        Recipients = new Recipients() {new Recipient {Address = recipientAddress}},
        Subject = subject
      };
      if (isBodyHtml)
      {
        postRequest.BodyHtml = body;
      }
      else
      {
        postRequest.BodyText = body;
      }
      var postResponse = reachMailApiClient.Easysmtp.Post(postRequest);

      return !postResponse.Failures;
    }
  }
}