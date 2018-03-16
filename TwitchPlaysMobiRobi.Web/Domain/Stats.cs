namespace TwitchPlaysMobiRobi.Web.Domain
{
  public class Stats
  {
    public int SecondsLeft { get; set; }
    public int StopVotes { get; private set; }
    public int LeftVotes { get; private set; }
    public int RightVotes { get; private set; }
    public int ForwardVotes { get; private set; }

    public void AddStopVote() => StopVotes++;
    public void AddLeftVote() => LeftVotes++;
    public void AddRightVote() => RightVotes++;
    public void AddForwardVote() => ForwardVotes++;

    public void ResetVoteCounts()
    {
      StopVotes = 0;
      LeftVotes = 0;
      RightVotes = 0;
      ForwardVotes = 0;
    }
  }
}
