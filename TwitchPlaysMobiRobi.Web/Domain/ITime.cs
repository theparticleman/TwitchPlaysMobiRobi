using System;

namespace TwitchPlaysMobiRobi.Web.Domain
{
  public interface ITime
  {
    DateTime Now { get; }
  }
}