
using System;

namespace TwitchPlaysMobiRobi.Domain
{
    public interface IVoteResultCache
    {
        VoteResult Vote { get; set; }
    }

    public class VoteResultCache : IVoteResultCache
    {
        public VoteResult Vote { get; set; }
    }

    public class VoteResult
    {
        public VoteResult(string vote)
        {
            Vote = vote;
            Id = Guid.NewGuid().ToString("N");
        }

        public string Vote { get; }
        public string Id { get; }
    }
}