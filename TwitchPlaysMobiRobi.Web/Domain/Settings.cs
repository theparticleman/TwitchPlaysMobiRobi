namespace TwitchPlaysMobiRobi.Web.Domain
{
  public interface ISettings
  {
    string MobiRobiBaseUrl { get; }
    int SecondsBetweenVotes { get; }
  }

  public class Settings : ISettings
  {
    public Settings()
    {
      MobiRobiBaseUrl = "http://mobirobi/";
      SecondsBetweenVotes = 10;
    }

    public string MobiRobiBaseUrl { get; set; }
    public int SecondsBetweenVotes { get; set; }
  }
}