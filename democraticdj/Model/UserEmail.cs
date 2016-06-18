namespace Democraticdj.Model
{
  public class UserEmail
  {
    public string Address { get; set; }
    public bool IsVerified { get; set; }
    public string PendingVerificationId { get; set; }

  }
}