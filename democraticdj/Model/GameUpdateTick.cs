namespace Democraticdj.Model
{
  public class GameUpdateTick
  {
    public MongoDB.Bson.ObjectId Id { get; set; }
    public string GameId { get; set; }
    public long GameTick { get; set; }
  }
}