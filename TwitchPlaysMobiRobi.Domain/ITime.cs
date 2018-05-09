using System;

namespace TwitchPlaysMobiRobi.Domain
{
  public interface ITime
  {
    DateTime Now { get; }
  }
}