using System;
using System.Diagnostics;
using System.Threading;
using RestSharp;

namespace TwitchPlaysMobiRobi.Web.Domain
{
    public class VoteTimer
    {
        private readonly IRestClient client;
        private readonly Stats stats;
        private readonly ITime time;
        private readonly ISettings settings;
        private readonly IVoteResultCache voteCache;
        private Thread thread;
        private bool running;
        private DateTime nextUpdateTime;

        public VoteTimer(Stats stats, ISettings settings, IRestClient client, IVoteResultCache voteCache, ITime time)
        {
            if (stats == null) throw new ArgumentException("stats cannot be null");
            if (client == null) throw new ArgumentException("client cannot be null");
            if (time == null) throw new ArgumentException("time cannot be null");
            if (voteCache == null) throw new ArgumentException("voteCache cannot be null");
            this.client = client;
            this.stats = stats;
            this.voteCache = voteCache;
            this.time = time;
            this.settings = settings;
        }

        public bool IsRunning
        {
            get { return thread.IsAlive; }
        }

        public void Start()
        {
            SetNextVoteTime();
            stats.SecondsLeft = settings.SecondsBetweenVotes;
            thread = new Thread(ThreadBody)
            {
                IsBackground = true
            };
            running = true;
            thread.Start();
        }

        private void SetNextVoteTime()
        {
            nextUpdateTime = time.Now.AddSeconds(settings.SecondsBetweenVotes);
        }

        private void ThreadBody()
        {
            while (running)
            {
                if (time.Now > nextUpdateTime)
                {
                    SendVote();
                    SetNextVoteTime();
                    stats.ResetVoteCounts();
                }
                UpdateSecondsLeft();
                Thread.Sleep(50);
            }
        }

        private void UpdateSecondsLeft()
        {
            stats.SecondsLeft = (int)(nextUpdateTime - time.Now).TotalSeconds;
        }

        private void SendVote()
        {
            if (stats.StopVotes <= 0 && stats.LeftVotes <= 0 && stats.RightVotes <= 0 && stats.ForwardVotes <= 0) return;
            var command = GetCommandWithHighestVotes();
            voteCache.Vote = new VoteResult(command);
            var url = settings.MobiRobiBaseUrl + command;
            var request = new RestRequest(url);
            var response = client.Execute(request);
        }

        private string GetCommandWithHighestVotes()
        {
            int maxVotes = Math.Max(Math.Max(Math.Max(stats.StopVotes, stats.LeftVotes), stats.RightVotes), stats.ForwardVotes);
            if (stats.StopVotes == maxVotes) return "stop";
            if (stats.LeftVotes == maxVotes) return "left/1.5";
            if (stats.RightVotes == maxVotes) return "right/1.5";
            return "forward/3";
        }

        public void Stop()
        {
            running = false;
            var stopwatch = Stopwatch.StartNew();
            while (IsRunning && stopwatch.ElapsedMilliseconds < 5000) Thread.Sleep(50);
        }
    }
}