using System;

namespace TwitchPlaysMobiRobi.Domain
{
  public class Time : ITime
  {
    public DateTime Now => DateTime.Now;
  }
}