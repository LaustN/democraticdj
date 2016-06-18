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

      var mailAccountKey = ConfigurationManager.AppSettings[reachMailAccountKey];
      var mailUserName = ConfigurationManager.AppSettings[reachMailUserName];

      var reachMailApiClient = Reachmail.Api.Create(mailAccountKey, mailUserName, "AMOKAMOK");
      var postResponse = reachMailApiClient.Easysmtp.Post(new DeliveryRequest
      {
        BodyText = body,
        FooterAddress = "laustn@gmail.com",
        FromAddress = "laustn@gmail.com",
        Recipients = new Recipients() { new Recipient { Address = recipientAddress } },
        Subject = subject
      });

      return !postResponse.Failures;
    }
  }
}