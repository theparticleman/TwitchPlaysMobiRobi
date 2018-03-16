using System;

namespace TwitchPlaysMobiRobi.Web.Domain
{
  public class Time : ITime
  {
    public DateTime Now => DateTime.Now;
  }
}