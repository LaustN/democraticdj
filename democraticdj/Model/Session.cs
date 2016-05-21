using System;
using Democraticdj.Services;

namespace Democraticdj.Model
{
  public class Session :IDisposable
  {
    public MongoDB.Bson.ObjectId Id { get; set; }
    public string SessionId { get; set; }
    public string GameId { get; set; }
    public string UserId { get; set; }

    public void Dispose()
    {
      StateManager.SetSession(this);
    }
  }
}