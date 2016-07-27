namespace Democraticdj.Model
{
  public class GameUpdateLock
  {
    public int UpdatingThreadRandomizedId { get; set; }
    public long UpdatingStartedTicks { get; set; }
  }
}